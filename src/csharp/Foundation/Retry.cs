using System;
using System.Threading.Tasks;

namespace Foundation
{
    public class Retry
    {
        public static async Task<TResult> ExecuteAsync<TResult>(Func<TResult> action, int retryMaxCount, TimeSpan retryInterval, Func<Exception, bool> judgeDoRetry)
        {
            //パラメータチェック
            if (retryMaxCount < 0)
            {
                //最大リトライ回数
                throw new ArgumentOutOfRangeException("retryFrequency");
            }
            if (retryInterval < TimeSpan.Zero)
            {
                //リトライ間隔
                throw new ArgumentOutOfRangeException("retryInterval");
            }
            if (action == null)
            {
                //処理
                throw new ArgumentNullException("action");
            }

            int retryCount = 0;

            while (true)
            {
                try
                {
                    return action();
                }
                catch(Exception e)
                {
                    if (retryCount++ >= retryMaxCount)
                    {
                        throw;
                    }

                    if (judgeDoRetry?.Invoke(e) ?? true)
                    {
                        //最大リトライ回数に達していない場合
                        await Task.Delay(retryInterval);
                    }
                    else {
                        throw;
                    }
                }
            }
        }

        public static async Task ExecuteAsync(Action action, int retryMaxCount, TimeSpan retryInterval, Func<Exception, bool> judgeDoRetry)
        {
            //パラメータチェック
            if (retryMaxCount < 0)
            {
                //最大リトライ回数
                throw new ArgumentOutOfRangeException("retryFrequency");
            }
            if (retryInterval < TimeSpan.Zero)
            {
                //リトライ間隔
                throw new ArgumentOutOfRangeException("retryInterval");
            }
            if (action == null)
            {
                //処理
                throw new ArgumentNullException("action");
            }

            int retryCount = 0;

            while (true)
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    if (retryCount++ >= retryMaxCount)
                    {
                        throw;
                    }

                    if (judgeDoRetry?.Invoke(e) ?? true)
                    {
                        //最大リトライ回数に達していない場合
                        await Task.Delay(retryInterval);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }
    }
}
