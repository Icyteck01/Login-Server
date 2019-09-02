using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace JHUI.Utils
{
    public class JBehavor
    {
        private List<JBehavor.MethodMono> dict = new List<JBehavor.MethodMono>();

        public void InvokeRepeating(Action methodName, float time, float repeatRate = 0.0f)
        {
            this.dict.Add(new JBehavor.MethodMono(time)
            {
                Function = methodName,
                time = time,
                repeatRate = repeatRate
            });
        }

        public void Dispose()
        {
            foreach (MethodMono mm in new List<JBehavor.MethodMono>(dict))
            {
                mm.Dispose();
            }

        }

        public class MethodMono
        {
            public float lastInvoket = 0.0f;
            public float time = 0.0f;
            public float repeatRate = 0.0f;
            public Action Function = (Action)null;
            private float repeted = 0.0f;
            private bool Disposed = false;
            public MethodMono(float time)
            {
                Thread x = new Thread(() =>
                {
                    this.DoSomethingEveryTenSeconds(time);
                });
                x.IsBackground = true;
                x.Start();
            }

            public void DoSomethingEveryTenSeconds(float _time)
            {
                int timex = (int)((double)_time * 1000.0);
                while (!Disposed)
                {
                    Thread.Sleep(timex);
                    this.Invoke();
                }
            }
            public void Invoke()
            {
                if ((double)this.repeatRate != 0.0)
                {
                    this.Function();
                    this.repeted = this.repeted + 1f;
                }
                else
                    this.Function();
            }
            public bool canRepat()
            {
                if ((double)this.repeatRate != 0.0)
                    return true;
                return (double)this.repeatRate < (double)this.repeted;
            }

            public void Dispose()
            {
                Disposed = true;
            }
        }
    }
}
