namespace CodingBlocksUptime.Test;

public class HttpResponseTests
{
    [Test]
    public void Test_Response_Success()
    {
        var info = HttpTester.GetResponseInfo("https://swharden.com/sitemap.xml");
        Console.WriteLine(info);
        info.Code.Should().Be(200);
        info.Size.Should().BeGreaterThan(0);
    }

    [Test]
    public void Test_Response_NotFound()
    {
        var info = HttpTester.GetResponseInfo("https://swharden.com/this-page-does-not-exist.html");
        Console.WriteLine(info);
        info.Code.Should().Be(404);
    }

    [Test]
    public void Test_Response_Timeout()
    {
        var info = HttpTester.GetResponseInfo("https://swharden.com/sitemap.xml", timeoutMsec: 1);
        Console.WriteLine(info);
        info.Code.Should().Be(524);
    }
}