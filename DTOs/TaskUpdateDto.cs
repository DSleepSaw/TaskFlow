namespace TaskFlow.DTOs
{
    public class TaskUpdateDto
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
    }
}
