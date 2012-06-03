namespace System
{
   public delegate TResult Func<TResult>();
   public delegate TResult Func<T1, TResult>(T1 param1);
   public delegate TResult Func<T1, T2, TResult>(T1 param1, T2 param2);
   public delegate TResult Func<T1, T2, T3, TResult>(T1 param1, T2 param2, T3 param3);
}