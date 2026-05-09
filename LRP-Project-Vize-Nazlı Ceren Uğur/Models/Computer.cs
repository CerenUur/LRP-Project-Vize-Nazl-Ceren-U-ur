public class Computer
{
    public int Id { get; set; }
    public string Specs { get; set; } // Marka, RAM vb. buraya yazılacak
    public string InventoryCode { get; set; }
    public int LaboratoryId { get; set; }

    // Yönergedeki "Sorumluluk Atama" için eklenenler:
    public string? StudentNo { get; set; }
    public string? StudentName { get; set; }
}