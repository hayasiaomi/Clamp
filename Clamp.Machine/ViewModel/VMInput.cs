namespace Clamp.Machine.Dto
{
    /// <summary>
    /// 输入参数：操作人
    /// </summary>
    public class VMInput
    {
        /// <summary>
        /// 操作人Id
        /// </summary>
        public int OperatorId { get; set; }

        /// <summary>
        /// 操作人姓名
        /// </summary>
        public string OperatorName { get; set; }
    }
}
