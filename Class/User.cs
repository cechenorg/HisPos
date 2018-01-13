using System.Collections.Generic;

namespace His_Pos.Class
{
    public class User
	{
        public string UserId { get; set; }
	    public string UserPassword { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserQuickName { get; set; }
        public string UserBirth { get; set; }
        public string UserAddress { get; set; }
        public string UserPhone { get; set; }
        public string UserIdentityNumber { get; set; }
        public string UserGender { get; set; }
        public string UserWorkPlace { get; set; }
        public string UserPosition { get; set; }
        public List<string> UserPosAuth { get; set; }
        public List<string> UserHisAuth { get; set; }
        public Leave UserLeave { get; set; }
    }
}
