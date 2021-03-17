using Worldex.Core.ApiModels;
using Worldex.Core.Entities.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.Chat
{
    public class ChatUserResponseView : BizResponseClass
    {
        public IReadOnlyList<ApplicationUser> Users { get; set; }
        public long PageNo { get; set; }
        public long PageSize { get; set; }
        public long TotalCount { get; set; }
    }

    public class CouterResponseView : BizResponseClass
    {
        public long Count { get; set; }
    }

    public class UserViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Reason { get; set; }

        [Required]
        public bool IsBlocked { get; set; }

    }

    public class UserChatViewModel
    {
        [Required]
        public string Username { get; set; }
        
        public int Page { get; set; }
    }

    public class BlockedUserViewModel
    {
        public bool IsBlocked { get; set; }
    }

    public class BlockedUserUpdate
    {
        public string Message { get; set; }
    }

    public class ChatHistoryViewModel : BizResponseClass
    {
        public IEnumerable<SortedChatHistory> Data { get; set; }
        public long TotalRecord { get; set; }
        public long Page { get; set; }
    }

    public class SortedChatHistory
    {
        public string Message { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Time { get; set; }
        public long Score { get; set; }
    }
}
