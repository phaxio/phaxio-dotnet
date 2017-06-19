Imports System.IO

Module Module1

    Sub Main()
        Dim phaxio = New PhaxioClient("secret", "key")

        Dim status = phaxio.Account.Status
        Console.WriteLine(String.Format("Balance: {0}", status.Balance))

        Dim fax1 = phaxio.Fax.Create(to:="8088675309", file:=New IO.FileInfo("form1234.pdf"))

        Dim faxConfig = New Dictionary(Of String, Object) From
        {
            {"To", "8088675309"},
            {"File", "form1234.pdf"}
        }
        Dim fax2 = phaxio.Fax.Create(faxConfig)

        Dim pdf1 = New IO.FileInfo("form1234.pdf")
        Dim pdf2 = New IO.FileInfo("form4321.pdf")

        Dim fax = phaxio.Fax.Create(to:="8088675309", files:=New List(Of IO.FileInfo) From {pdf1, pdf2}, callerId:="2125552368")

        Dim fax3 = phaxio.Fax.Retrieve("1234")

        fax3.Resend()
        fax3.Cancel()
        fax3.File.Delete()
        fax3.Delete()

        Dim faxes = phaxio.Fax.List(createdBefore:=DateTime.Now)

        Dim areaCodes1 = phaxio.Public.AreaCode.List()

        Dim areaCodes2 = phaxio.Public.AreaCode.List(tollFree:=True)

        Dim areaCodes3 = phaxio.Public.AreaCode.List(state:="MA", country:="US")

        Dim areaCodes4 = phaxio.Public.AreaCode.List(tollFree:=True, state:="MA", country:="US")

        Dim newNumber1 = phaxio.PhoneNumber.Create("808", "1")

        Dim newNumber2 = phaxio.PhoneNumber.Create("808", "1", callbackUrl:="https://example.com/callback")

        Dim numbers1 = phaxio.PhoneNumber.List()

        Dim numbers2 = phaxio.PhoneNumber.List(countryCode:="1")

        Dim numbers3 = phaxio.PhoneNumber.List(areaCode:="808", countryCode:="1")

        Dim number = phaxio.PhoneNumber.Retrieve("8088675309")
        number.Release()

        Dim phaxCode1 = phaxio.PhaxCode.Create()

        Dim phaxCode2 = phaxio.PhaxCode.Create("code-for-form1234")

        File.WriteAllBytes("C:\temp\phaxCode.png", phaxCode1.Png)

        Dim phaxCode = phaxio.PhaxCode.Retrieve("1234")

        Dim png = phaxCode.Png

        Dim supportedCountries = phaxio.Public.SupportedCountry.List()

        Dim testFax1 = phaxio.Fax.TestRecieveCallback(New FileInfo("test-fax.pdf"))

        Dim testFax2 = phaxio.Fax.TestRecieveCallback(New FileInfo("test-fax.pdf"), to:="8088675309")

        Dim testFax3 = phaxio.Fax.TestRecieveCallback(New FileInfo("test-fax.pdf"), from:="2125552368")
    End Sub

End Module
