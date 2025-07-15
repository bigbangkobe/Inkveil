public class PropertyInfo
{
    public int propertyID { get; set; }
    public string propertyName { get; set; }
    public string propertyDes { get; set; }
    public int propertyGrade { get; set; }
    public string imagePath { get; set; }
    public int number { get; set; } = 0;

    public PropertyInfo() { }

    public PropertyInfo(int propertyID, string propertyName, string propertyDes, int propertyGrade, string imagePath, int number)
    {
        this.propertyID = propertyID;
        this.propertyName = propertyName;
        this.propertyDes = propertyDes;
        this.propertyGrade = propertyGrade;
        this.imagePath = imagePath;
        this.number = number;
    }
}
