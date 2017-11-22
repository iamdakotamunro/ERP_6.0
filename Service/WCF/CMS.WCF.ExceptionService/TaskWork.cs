using Framework.Common.Task;

namespace CMS.WCF.ExceptionService
{
    public class TaskWork
    {
        private static readonly Tasker _tasker;

        static TaskWork()
        {
            _tasker = new Tasker();

            _tasker.Add("处理还未发送异常", 10, () => Process.ProcessNoSend(Config.ReadQuantity));
            _tasker.Add("处理异常超时5次失败", 100, () => Process.ProcessFailure(Config.ReadQuantity, 5));
            _tasker.Add("处理异常超时10次失败", 200, () => Process.ProcessFailure(Config.ReadQuantity, 10));
            _tasker.Add("处理异常超时20次失败", 600, () => Process.ProcessFailure(Config.ReadQuantity, 20));
            _tasker.Add("删除处理异常成功执行的记录", 3600, Process.ProcessDeleteSuccess);
        }

        public static void Start()
        {
            _tasker.Start();
        }

        public static void Stop()
        {
            _tasker.Stop();
        }
    }
}
