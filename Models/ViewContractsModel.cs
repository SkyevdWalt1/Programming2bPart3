using System.Reflection.Metadata;

public class ViewContractsModel {
    public Dictionary<string, List<ClaimDetails>> Claims { get; set; } = new Dictionary<string, List<ClaimDetails>>();
}

public class ClaimDetails {
    public int ID {
        get; set;
    }
    public string Title {
        get; set;
    }
    public decimal HourlyRate {
        get; set;
    }
    public int Hours {
        get; set;
    }
    public DateTime StartDate {
        get; set;
    }
    public DateTime EndDate {
        get; set;
    }
    public string Status {
        get; set;
    }
    public string Notes {
        get; set;
    } // Assuming a single note

    public decimal Total => this.Hours * this.HourlyRate;

    public List<Document> Documents { get; set; } = new List<Document>();
}


public class Document {
    public string FileName {
        get; set;
    }
    public byte[] Data {
        get; set;
    }
    public string FileType {
        get; set;
    }
}
