using System.ComponentModel.DataAnnotations;

namespace OnlineVotingSystem.Models
{
    public class Poll
    {
        public int Id { get; set; }

        [Required, StringLength(200)]
        [Display(Name = "Question")]
        public string Question { get; set; } = string.Empty;

        [Display(Name = "Is Active?")]
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<PollOption> Options { get; set; } = new List<PollOption>();
    }
}
