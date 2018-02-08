using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElasticSearch_Sample.Models;
using Microsoft.AspNetCore.Mvc;
using Nest;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ElasticSearch_Sample.Controllers
{
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        public ProductsController()
        {
            var configuration = new ConnectionSettings(new Uri("http://172.17.0.3:9200")).DefaultIndex("product");
            client  = new ElasticClient(configuration);
        }

        ElasticClient client;

        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value3" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            var response = client.Search<Product>(s=>s
                                                  .Index("product-es")
                                                 .From(0)
                                                 .Size(10)
                                                 .Query(q=>q
                                                        .Match(m=>m
                                                               .Field(f=>f.ProductName)
                                                               .Query("Iphone 7"))));
            return response.Documents.FirstOrDefault().ProductName;
        }

        [HttpGet]
        [Route("~/api/products/getwithexplainusage/")]
        public ISearchResponse<Product> GetWithExplainUsage(string name)
        {
            var searchRequest = new SearchRequest<Product> 
            { 
                Explain = true
            };
            var documents=client.Search<Product>(s=>s.Index("product-es").Query(x=>x.Match(m=>m.Field(f=>f.ProductName).Query(name))).Explain(
            ));
            return documents;
        }

        [HttpGet]
        [Route("~/api/products/GetWithFieldsUsage/")]
        public ISearchResponse<Product> GetWithFieldsUsage(string name)
        {
            var searchRequest = new SearchRequest<Product>
            {
                Explain = true
            };
            var documents = client.Search<Product>(x=>x.StoredFields(f=>f.Field(y=>y.ProductName))
                                                   .Index("product-es").Query(r => r.Match(m => m.Field(f => f.ProductName).Query(name))));
            return documents;
        }

        [HttpGet]
        [Route("~/api/products/GetWithHighLightsUsage/")]
        public ISearchResponse<Product> GetWithHighLightsUsage(string name)
        {
            var searchRequest = new SearchRequest<Product>
            {
                Explain = true
            };
            var documents = client.Search<Product>(x => x.StoredFields(f => f.Field(y => y.ProductName))
                                                   .Index("product-es")
                                                   .Query(r => r.Match(m => m.Field(f => f.ProductName.Suffix("standart")).Query(name)))
                                                   .Highlight(h=>
                                                              h.PreTags("<b>")
                                                              .PostTags("</b>")
                                                              .Encoder(HighlighterEncoder.Html)
                                                              .Fields(f=>
                                                                      f.Field(p=>p.ProductName.Suffix("standart"))
                                                                        .Type("plain")
                                                                        
            
                                                                     ))
                                                  );
            return documents;
        }

        // POST api/values
        [HttpPost]
        public string Post(string productName,string categoryName,int id)
        {
             var product = new Models.Product
             {
                CategoryName = categoryName,
                ProductName = productName,
                 Id = id
             };

             var r = client.Index<Product>(product, i => i.Index("product-es"));
            return r.Id;
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public Result Put(int id, [FromBody]string value)
        {
            var product = new Product() { Id=id,ProductName=value };
            var response = client.Update<Product>(DocumentPath<Product>.Id(7),u => u.Index("product").Doc(product));
            return response.Result;
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
