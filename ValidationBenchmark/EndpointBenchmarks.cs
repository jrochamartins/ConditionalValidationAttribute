using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;

namespace ValidationBenchmark
{
    // O MemoryDiagnoser é essencial para ver o uso de RAM (Gen0, Gen1, Gen2, Allocated)
    [MemoryDiagnoser]
    public class EndpointBenchmarks
    {
        private WebApplicationFactory<ValidationSample.Program> _factory;
        private HttpClient _client;
        private JsonContent _payload;

        [GlobalSetup]
        public void Setup()
        {
            // Inicializa a API em memória
            _factory = new WebApplicationFactory<ValidationSample.Program>();
            _client = _factory.CreateClient();

            // Cria o payload uma única vez para não afetar o benchmark
            var requestData = new { Name = "", Validate = true, NameType = 1 };
            _payload = JsonContent.Create(requestData);
        }

        [Benchmark(Baseline = true)]
        public async Task<HttpResponseMessage> EndpointWithDinamicCoreRequiredIf()
        {
            return await _client.PostAsync("/Validation/WithDinamicCoreRequiredIf", _payload);
        }

        [Benchmark]
        public async Task<HttpResponseMessage> EndpointWithDynamicExpressoRequiredIf()
        {
            return await _client.PostAsync("/Validation/WithDynamicExpressoRequiredIf", _payload);
        }

        [Benchmark]
        public async Task<HttpResponseMessage> EndpointWithZExpresssionsRequiredIf()
        {
            return await _client.PostAsync("/Validation/WithZExpresssionsRequiredIf", _payload);
        }

        [Benchmark]
        public async Task<HttpResponseMessage> EndpointValidateIf()
        {
            return await _client.PostAsync("/Validation/ValidateIf", _payload);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _client?.Dispose();
            _factory?.Dispose();
        }
    }
}
