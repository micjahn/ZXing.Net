/*
 * Copyright 2010 ZXing authors
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using ZXing.Common;

namespace ZXing.OneD
{

    /// <summary>
    /// <p>Decodes PharmaCode</p>
    /// * @author Ruslan Karachun
    /// </summary>
    public sealed class PharmaCodeReader : OneDReader
    {
        private static bool isBlack = true;
        private static bool isWhite = false;

        internal class PixelInterval
        {
            public PixelInterval(bool c, int l)
            {
                Color = c;
                Length = l;
            }

            public bool Color { get; private set; }
            public int Length { get; private set; }
            public int Similar { get; private set; }
            public int Small { get; private set; }
            public int Large { get; private set; }

            public void incSimilar()
            {
                Similar++;
            }

            public void incSmall()
            {
                Small++;
            }

            public void incLarge()
            {
                Large++;
            }
        }

        public static double mean(double[] m)
        {
            double sum = 0;
            int l = m.Length;
            for (int i = 0; i < l; i++)
            {
                sum += m[i];
            }

            return sum / m.Length;
        }

        public override Result decodeRow(int rowNumber, BitArray row, IDictionary<DecodeHintType, object> hints)
        {
            var gaps = new List<PixelInterval>();

            bool color = row[0];
            int end = row.Size;
            int num = 0;

            for (int i = 0; i < end; i++)
            {
                bool currentColor = row[i];
                if (currentColor == color)
                {
                    num++;
                }
                else
                {
                    gaps.Add(new PixelInterval(color, num));
                    color = currentColor;
                    num = 1;
                }
            }

            gaps.Add(new PixelInterval(color, num));

            int gaps_length = gaps.Count;
            for (int i = 0; i < gaps_length; i++)
            {
                PixelInterval primary = gaps[i];
                bool p_color = primary.Color;
                int p_num = primary.Length; // количество пикселей
                for (int j = 0; j < gaps_length; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    int s_num = gaps[j].Length;
                    bool s_color = gaps[j].Color;
                    double multiplier = (p_num > s_num) ? ((double) p_num / s_num) : ((double) s_num / p_num);
                    //System.out.println("multiplier: " + multiplier);
                    if ((p_color == isWhite) && (s_color == isWhite))
                    {
                        // WHITE WHITE
                        if (multiplier <= 1.2222)
                        {
                            primary.incSimilar();
                        }
                        else
                        {
                            //System.out.println("NOT SIMILAR");
                        }
                    }
                    else if ((p_color == isWhite) && (s_color == isBlack))
                    {
                        // WHITE BLACK
                        if ((multiplier > 1.5) && (multiplier < 3.6667) && (p_num > s_num))
                        {
                            // White and small black
                            primary.incSimilar();
                        }
                        else if ((multiplier > 1.2727) && (multiplier < 2.7778) && (p_num < s_num))
                        {
                            // White and large black
                            primary.incSimilar();
                        }
                        else
                        {
                            //System.out.println("NOT SIMILAR");
                        }
                    }
                    else if ((p_color == isBlack) && (s_color == isWhite))
                    {
                        // BLACK WHITE
                        if ((multiplier > 1.5) && (multiplier < 3.6667) && (p_num < s_num))
                        {
                            // Small black and white
                            primary.incSimilar();
                            primary.incSmall();
                        }
                        else if ((multiplier > 1.2727) && (multiplier < 2.7778) && (p_num > s_num))
                        {
                            // large black and white
                            primary.incSimilar();
                            primary.incLarge();
                        }
                        else
                        {
                            //System.out.println("NOT SIMILAR");
                        }
                    }
                    else if ((p_color == isBlack) && (s_color == isBlack))
                    {
                        // BLACK BLACK
                        if ((multiplier > 2.3333) && (multiplier < 4.6667))
                        {
                            primary.incSimilar();
                            if (p_num > s_num)
                            {
                                primary.incLarge();
                            }
                            else
                            {
                                primary.incSmall();
                            }
                        }
                        else if (multiplier < 2)
                        {
                            primary.incSimilar();
                        }
                        else
                        {
                            //System.out.println("NOT SIMILAR");
                        }
                    }
                    else
                    {
                        //System.out.println("UNKNOWN COLORS");
                    }
                } // j
            } // i

            var iResult = finalProcessing(gaps);
            if (iResult == null || (iResult < 3) || (iResult > 131070))
            {
                return null;
            }

            String resultString = iResult.ToString();
//Counter counter = Counter.getInstance(25);
//counter.addCode(iResult);

//    String sRowNumber = Integer.toString(rowNumber);
//    String url = "https://dev.aptinfo.net/p/" + resultString;
//    final HttpRequestFactory requestFactory = new NetHttpTransport().createRequestFactory();
//new Thread(new Runnable()
//{
//    @Override
//        public void run()
//    {
//        try
//        {
//            HttpRequest request = requestFactory.buildGetRequest(new GenericUrl(url));
//            HttpResponse httpResponse = request.execute();
//        }
//        catch (IOException e)
//        {
//            //e.printStackTrace();
//        }
//    }
//}).start();

//    if ( ! counter.isCodeValid(iResult) ) {
//        throw NotFoundException.getNotFoundInstance();
//    }

            float left = 0.0f;
            float right = (float) (end - 1);
            return new Result(
                resultString,
                null,
                new ResultPoint[]
                {
                    new ResultPoint(left, rowNumber),
                    new ResultPoint(right, rowNumber)
                },
                BarcodeFormat.PHARMA_CODE
            );

        }


        private int? finalProcessing(List<PixelInterval> gaps)
        {
            int l = gaps.Count;
            double[]
                similars = new double[l];
            for (int i = 0; i < l; i++)
            {
                similars[i] = (double) gaps[i].Similar;
            }

            double dMean = mean(similars);
            bool inProgress = false;
            String fStr = "";
            String cStr = "";
            for (int i = 0; i < l; i++)
            {
                PixelInterval gap = gaps[i];
                bool color = gap.Color;
                double sim = (double) gap.Similar;
                if ((color == isWhite) && (!inProgress) && (sim < dMean))
                {
                    //System.out.println("start");
                    inProgress = true;
                    continue;
                }

                if (inProgress && (sim < dMean))
                {
                    //System.out.println("Similar is " + sim + " < " + dMean + " => BREAK");
                    if (color == isBlack)
                    {
                        return null;
                    }

                    if ((color == isWhite) && ((i + 1) != l))
                    {
                        return null;
                    }
                }

                if (((i + 1) == l) && (gap.Color == isBlack))
                {
                    //System.out.println("last gap");
                    return null;
                }

                if (inProgress && (color == isBlack))
                {
                    if (gap.Large > gap.Small)
                    {
                        fStr += '1';
                        cStr += '#';
                    }
                    else
                    {
                        fStr += '0';
                        cStr += '=';
                    }
                }
            }

            //System.out.println("Str: "+ fStr +" "+ cStr);
            String stg2 = '1' + fStr;
            int ret_val = Convert.ToInt32(stg2, 2) - 1;
            //System.out.println(ret_val);
            return ret_val;
        }
    }
}
