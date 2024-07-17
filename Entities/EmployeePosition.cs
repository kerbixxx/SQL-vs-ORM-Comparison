namespace SQL.Entities
{
    public class EmployeePosition
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int? PositionId { get; set; } // Используем nullable int для PositionId, так как некоторые сотрудники могут не иметь позиции
        public string PositionName { get; set; } // Также nullable, так как позиция может отсутствовать
    }
}
