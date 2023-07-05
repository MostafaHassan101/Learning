namespace Domain
{
    public class Employee : Person
    {
        public int Salary { get; set; }
        public DateTime Birthday { get; set; }
        public bool IsManager { get; set; }
        public List<Employee> Employees { get; set; }
        public TimeSpan Hours { get; set; }
    }
}