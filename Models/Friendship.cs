using System;
using System.Collections.Generic;

namespace CommunityBookLovers
{
    public partial class Friendship
    {
        public int FriendshipId { get; set; }

        public int UserId { get; set; }  // Убрал nullable (?), так как отношения должны быть определены

        public int FriendId { get; set; }  // Убрал nullable (?)

        public bool IsAccepted { get; set; } = false;

        public DateTime RequestDate { get; set; } = DateTime.Now;  // Убрал internal set и добавил значение по умолчанию

        public virtual User Friend { get; set; } = null!;  // Убрал nullable и добавил null-forgiving operator

        public virtual User User { get; set; } = null!;  // Убрал nullable
    }
}