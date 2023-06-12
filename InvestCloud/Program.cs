// See https://aka.ms/new-console-template for more information
using Newtonsoft.Json;
using RestSharp;
using System.Diagnostics;

const string ApiKey = "https://recruitment-test.investcloud.com/api/numbers/";
const string Column = "col";
const string Row = "row";
const int sizeOfArray = 1000;

int dimensions = 0;

Console.WriteLine("Getting the size of dataset!");
dimensions = GetDatasetSize(sizeOfArray);
long[,] dataSetA = new long[dimensions, dimensions];
long[,] dataSetB = new long[dimensions, dimensions];
dataSetA = GetDatasetData("A", dataSetA);
dataSetB = GetDatasetData("B", dataSetB);
CrossProduct(dataSetA, dataSetB);
Stopwatch timer = new Stopwatch();
timer.Start();
long[,] dataSetR = StrassenMultiplication(dataSetA, dataSetB);
timer.Stop();
TimeSpan ts = timer.Elapsed;
Console.WriteLine("Strassen Multiplication took: " + ts.Seconds + " seconds");
HashOutput(String.Join(" ", dataSetR.Cast<long>()));

static int GetDatasetSize(int size)
{
    var client = new RestClient(ApiKey);
    var request = new RestRequest($"init/{size}");
    var response = client.Execute(request);
    dynamic val;

    if (response.StatusCode == System.Net.HttpStatusCode.OK)
    {
        string? rawData = response.Content;
        Rootobject obj = JsonConvert.DeserializeObject<Rootobject>(rawData);
        val = obj.Value;
        return (int) val;
    }
    else
        return 0;
}

static long[,] GetDatasetData(string dataset, long[,] dataSet)
{

    var client = new RestClient(ApiKey);
    Console.WriteLine("Initializing dataset: " + dataset);
    for (int i = 0; i < dataSet.GetLength(0); i++)
    {
        var request = new RestRequest($"{dataset}/{Row}/{i}");
        var response = client.Execute(request);
        
        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var rawData = response.Content;
            Rootobject obj = JsonConvert.DeserializeObject<Rootobject>(rawData);
            int j = 0;
            foreach(var w in obj.Value)
            {
                dataSet[i,j] = (long)w;
                j++;
            }
        }
    }
    return dataSet;
}

static void CrossProduct(long[,] dataSetA, long[,] dataSetB)
{
    Stopwatch timer = new System.Diagnostics.Stopwatch();
    timer.Start();
    long[,] result = new long[dataSetA.GetLength(0), dataSetB.GetLength(0)];

    for (int i = 0; i < dataSetA.GetLength(0); i++)
    {
        for (int j = 0; j < dataSetB.GetLength(1); j++)
        {
            long temp = 0;
            for (int k = 0; k < dataSetA.GetLength(0); k++)
            {
                temp += dataSetA[i, k] * dataSetB[k, j];
            }
            result[i, j] = temp;
        }
    }
    timer.Stop();
    TimeSpan ts = timer.Elapsed;
    Console.WriteLine("Cross Prod took: " + ts.Seconds + " seconds");
    HashOutput(String.Join(" ", result.Cast<long>()));
}

static void HashOutput(string postData)
{
    string hashedString;
    
    using(System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
        byte[] input = System.Text.Encoding.UTF8.GetBytes(postData.Trim().Replace(" ", ""));
        byte[] output = md5.ComputeHash(input);
        hashedString = Convert.ToHexString(output); 
            }
    PostResultData(hashedString);

}

static long[,] StrassenMultiplication(long[,] dataSetA, long[,] dataSetB)
{
    long sizeA = dataSetA.GetLength(0);

    long[,] dataSetC = new long[dataSetA.GetLength(0), dataSetB.GetLength(0)];

    if(sizeA == 1)
        dataSetC[0,0] = dataSetA[0,0] * dataSetB[0,0];
    else
    {
        long[,] A11 = new long[sizeA / 2, sizeA / 2];
        long[,] A12 = new long[sizeA / 2, sizeA / 2];
        long[,] A21 = new long[sizeA / 2, sizeA / 2];
        long[,] A22 = new long[sizeA / 2, sizeA / 2];
        long[,] B11 = new long[sizeA / 2, sizeA / 2];
        long[,] B12 = new long[sizeA / 2, sizeA / 2];
        long[,] B21 = new long[sizeA / 2, sizeA / 2];
        long[,] B22 = new long[sizeA / 2, sizeA / 2];

        SplitDatasets(dataSetA, A11, 0, 0);
        SplitDatasets(dataSetA, A12, 0, sizeA/2);
        SplitDatasets(dataSetA, A21, sizeA / 2, 0);
        SplitDatasets(dataSetA, A22, sizeA / 2, sizeA / 2);

        SplitDatasets(dataSetB, B11, 0, 0);
        SplitDatasets(dataSetB, B12, 0, sizeA / 2);
        SplitDatasets(dataSetB, B21, sizeA / 2, 0);
        SplitDatasets(dataSetB, B22, sizeA / 2, sizeA / 2);

        long[,] D1 = StrassenMultiplication(AddDatasets(A11, A22), AddDatasets(B11, B22));
        long[,] D2 = StrassenMultiplication(AddDatasets(A21, A22), B11);
        long[,] D3 = StrassenMultiplication(A11, SubtractDatasets(B12, B22));
        long[,] D4 = StrassenMultiplication(A22, SubtractDatasets(B21, B11));
        long[,] D5 = StrassenMultiplication(AddDatasets(A11, A12), B22);
        long[,] D6 = StrassenMultiplication(SubtractDatasets(A21, A11), AddDatasets(B11, B12));
        long[,] D7 = StrassenMultiplication(SubtractDatasets(A12, A22), AddDatasets(B21, B22));

        long[,] C11 = AddDatasets(SubtractDatasets(AddDatasets(D1, D4), D5), D7);
        long[,] C12 = AddDatasets(D3, D5);
        long[,] C21 = AddDatasets(D2, D4);
        long[,] C22 = AddDatasets(SubtractDatasets(AddDatasets(D1, D3), D2), D6);

        CombineDatasets(C11, dataSetC, 0, 0);
        CombineDatasets(C12, dataSetC, 0, sizeA/2);
        CombineDatasets(C21, dataSetC, sizeA/2, 0);
        CombineDatasets(C22, dataSetC, sizeA / 2, sizeA / 2);

    }
    return dataSetC;
}

static long[,] SubtractDatasets(long[,] a, long[,] b)
{
    long len = a.GetLength(0);
    long[,] result = new long[len, len];
    for (int i = 0; i < len; i++)
    {
        for (int j = 0; j < len; j++)
        {
            result[i, j] = a[i, j] - b[i, j];
        }
    }
    return result;
}

static long[,] AddDatasets(long[,] a, long[,] b)
{
    long len = a.GetLength(0);
    long[,] result = new long[len, len];
    for (int i = 0; i < len; i++)
    {
        for (int j = 0; j < len; j++)
        {
            result[i,j] = a[i, j] + b[i, j];
        }
    }
    return result;
}

static void SplitDatasets(long[,] originaldataset, long[,] partitionedDataset, long i, long j)
{
    for (long a = 0, b = i; a < partitionedDataset.GetLength(0); a++, b++)
    {
        for (long c = 0, d = j; c < partitionedDataset.GetLength(0); c++, d++)
        {
            partitionedDataset[a, c] = originaldataset[b, d];
        }
    }
}

static void CombineDatasets(long[,] partitionedDataset, long[,] originalDataset, long i, long j)
{
    for (long a = 0, b = i; a < partitionedDataset.GetLength(0); a++, b++)
    {
        for (long c = 0, d = j; c < partitionedDataset.GetLength(0); c++, d++)
        {
            originalDataset[b, d] = partitionedDataset[a, c];
        }
    }
}

static void  PostResultData(string hashedStr)
{
    var client = new RestClient(ApiKey);
    var request = new RestRequest("validate", Method.Post);
    request.AddParameter("Value", hashedStr);
    request.AddHeader("Content-Type", "application/json; charset=utf-8");
    var response = client.Execute(request);

    dynamic val;

    if (response.StatusCode == System.Net.HttpStatusCode.OK)
    {
        string? rawData = response.Content;
        Console.WriteLine(rawData);
        Rootobject obj = JsonConvert.DeserializeObject<Rootobject>(rawData);
        val = obj.Success;
        Console.WriteLine(val);
    }
}
public class Rootobject
{
    public dynamic Value { get; set; }
    public object Cause { get; set; }
    public bool Success { get; set; }
}
