using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineVotingSystem.Models
{
    public class PollOption
    {
        public int Id { get; set; }

        [Required]
        public int PollId { get; set; }

        [ForeignKey(nameof(PollId))]
        public Poll? Poll { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Option Text")]
        public string Text { get; set; } = string.Empty;

        [Display(Name = "Votes")]
        public int VoteCount { get; set; }
    }
}
