


using CSharConverter.Converter;

class Program
{
    static void Main()
    {
        IConverter type = new TypeScriptConverter();
     
        var stringVale = @"public class PersonDto     {         public string Name { get; set; }         public int Age { get; set; }         public string Gender { get; set; }         public long? DriverLicenceNumber { get; set; }         public List<Address> Addresses { get; set; }         public class Address         {             public int StreetNumber { get; set; }             public string StreetName { get; set; }             public string Suburb { get; set; }             public int PostCode { get; set; }         }     }";

        Console.WriteLine(type.Convert(stringVale));
    }
}
