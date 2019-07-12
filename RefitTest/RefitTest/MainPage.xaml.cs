using Refit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.DependencyInjection;

namespace RefitTest
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage :ContentPage
    {
        private const string baseUrl = "https://reqres.in";
        private StringContent content;
        private IReqResApi _reqResApi;
        private ServiceCollection _services;
        private HttpClient _client;
        private ServiceProvider _serviceProvider;
        private IReqResApi _reqResApiHttpClientBuilder;
        private int numberOfCalls => 100;

        public MainPage()
        {
            InitializeComponent();

            content = new StringContent(
                " {\"name\": \"morpheus\",\"job\": \"zion resident\",\"updatedAt\": \"2019-07-12T11:28:22.703Z\"}",
                Encoding.UTF8,
                "application/json");

            _reqResApi = RestService.For<IReqResApi>(baseUrl);

            _services = new ServiceCollection();
            _services.AddRefitClient<IReqResApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl));
            _serviceProvider = _services.BuildServiceProvider();
            _reqResApiHttpClientBuilder = _serviceProvider.GetRequiredService<IReqResApi>();

            _client = new HttpClient(new HttpClientHandler())
            {
                BaseAddress = new Uri(baseUrl)
            };
        }


        private async void Button_Pressed(object sender, EventArgs e)
        {
            Refittext.Text = RefitHttpClientFactorytext.Text = Flurltext.Text = Refittext.Text = HttpClienttext.Text = "Processing...";
            var taskList = new List<Task>
            {
                RunHttpClient(),
                RunRefit(),
                RunRefitHttpClientFactory(),
                RunFlurl()
            };

            await Task.WhenAll(taskList);
        }

        private async Task RunRefit()
        {
            try
            {
                var taskList = new List<Task>();

                for (int i = 0; i < numberOfCalls; i++)
                {
                    taskList.Add(_reqResApi.Send(content));
                }

                await Task.WhenAll(taskList);

                Refittext.Text = $"Made {numberOfCalls} Refit Calls";
            }
            catch (Exception ex)
            {
                Refittext.Text = ex.Message;
            }
        }

        private async Task RunRefitHttpClientFactory()
        {
            try
            {
                var taskList = new List<Task>();

                for (int i = 0; i < numberOfCalls; i++)
                {
                    taskList.Add(_reqResApiHttpClientBuilder.Send(content));
                }

                await Task.WhenAll(taskList);

                RefitHttpClientFactorytext.Text = $"Made {numberOfCalls} Refit HttpClient Factory Calls";
            }
            catch (Exception ex)
            {
                RefitHttpClientFactorytext.Text = ex.Message;
            }
        }

        private async Task RunHttpClient()
        {
            try
            {
                var taskList = new List<Task>();

                for (int i = 0; i < numberOfCalls; i++)
                {
                    taskList.Add(_client.PutAsync($"/api/users/2", content));
                }

                await Task.WhenAll(taskList);
                HttpClienttext.Text = $"Made {numberOfCalls} HttpClient Calls";
            }
            catch (Exception ex)
            {
                HttpClienttext.Text = ex.Message;
            }
        }
        private async Task RunFlurl()
        {
            try
            {
                var taskList = new List<Task>();

                for (int i = 0; i < numberOfCalls; i++)
                {
                    taskList.Add(baseUrl
                        .AppendPathSegment($"/api/users/2")
                        .PutAsync(content)
                        );
                }

                await Task.WhenAll(taskList);
                Flurltext.Text = $"Made {numberOfCalls} Flurl Calls";
            }
            catch (Exception ex)
            {
                Flurltext.Text = ex.Message;
            }
        }
    }

    public interface IReqResApi
    {
        [Put("/api/users/2")]
        Task<HttpResponseMessage> Send([Body]StringContent check);
    }
}
