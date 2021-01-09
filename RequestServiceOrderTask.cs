using Newtonsoft.Json;

namespace DebugLogUploadMobile
{
    public class RequestServiceOrderTask
    {
        public string UserId { get; set; }
        public int Id { get; set; }
        public int ServiceOrderId { get; set; }
        public double Xp { get; set; }
        public double Yp { get; set; }
        public string Tasks { get; set; }
        public string TasksStatus { get; set; }
        public string TasksFeedback { get; set; }
        public int Target { get; set; }
        public string TargetIdAliasLabel { get; set; }
        public string TargetIdAlias { get; set; }
        public int Status { get; set; }
        public string PhotosWork { get; set; }
        public bool Exclude { get; set; }
        public string Extra { get; set; }
        public string PhotoSlot { get; set; }
        public int CommitedStatus { get; set; }
        public string ItemData { get; set; }
        

    }
}