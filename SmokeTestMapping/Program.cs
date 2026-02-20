using System.Reflection;
using System.Runtime.Serialization;
using NPPContractManagement.API.Services;

static (string? mapped, bool excluded, string reason) InvokeMap(object svc, string input)
{
    var mi = typeof(ContractService).GetMethod("MapPriceTypeWithNearest", BindingFlags.NonPublic | BindingFlags.Instance);
    if (mi == null) throw new InvalidOperationException("Method not found");
    var res = mi.Invoke(svc, new object?[] { input ?? string.Empty })!;
    // tuple: (string? Mapped, bool Excluded, string Reason)
    var t = ((string?, bool, string))res;
    return (t.Item1, t.Item2, t.Item3);
}

var inputs = new[]
{
    "Discontinued",
    "Discountinued",   // misspelled → exclude
    "Product Discontinued",
    "Suspnded",        // misspelled → Suspended
    "Product Suspended",
    "Published List Price at Time of Purchase",
    "List at time of purchase / No Bid",
    "Guaranted Price", // misspelled → Contract Price at Time of Purchase
    "Guaranteed Price",
    "Random Unknown Type"
};

// Create an uninitialized instance to invoke the private method without dependencies
var svc = (ContractService)FormatterServices.GetUninitializedObject(typeof(ContractService));

Console.WriteLine("Smoke test: PriceType nearest-match mapping\n");
foreach (var input in inputs)
{
    var (mapped, excluded, reason) = InvokeMap(svc, input);
    Console.WriteLine($"raw='{input}' => mapped='{mapped ?? "<null>"}', excluded={excluded}, reason='{reason}'");
}

