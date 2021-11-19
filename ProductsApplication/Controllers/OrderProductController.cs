using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductsApplication.Repository;

namespace ProductsApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "JwtBearer")]
    public class OrderProductController : ControllerBase
    {
        #region Fields

        private readonly OrderProductRepo orderProductRepo;
        private readonly IMapper _mapper;
       
        #endregion
        public OrderProductController(ApplicationContext prodContext, IMapper mapper)
        {
            _mapper = mapper;
            orderProductRepo = new OrderProductRepo(prodContext);
        }

        #region Order

        [HttpGet("get-UserOrders/{userId}")]
        public IActionResult GetUserOrders(string userId)
        {
            try
            {
                if(string.IsNullOrEmpty(userId))
                    return Unauthorized("Invalid Authentication");
                var ordersdto = new List<OrderDto>();
                var ordrs = orderProductRepo.GetUserOrders(userId);
                var xx = ordrs.GroupBy(o => o.OrderId).Select(p => new { Orders = p.ToList() });
              //  var prodLst = _mapper.Map<List<ProductDto>>(ordrs.GroupBy(o=>o.OrderId).Select(p=>new {Products= p.ToList()}));
                foreach (var item in xx)
                {
                    var orderdto = new OrderDto();
                    var prodLst= _mapper.Map<List<ProductDto>>(item.Orders.Select(o => o.Product).ToList()); 
                    orderdto.OrderId = item.Orders.FirstOrDefault().Order.Id;
                    orderdto.Products = prodLst;
                    orderdto.orderDate = item.Orders.FirstOrDefault().Order.Date;
                    orderdto.TotalQuantity = item.Orders.Select(p => p.Quantity).Sum();
                    orderdto.TotalPrice = prodLst.Select(p => p.Price).Sum();
                    ordersdto.Add(orderdto);
                }
               
                return Ok(ordersdto);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet("get-Order/{id}")]
        public IActionResult GetOrder(string id)
        {
            try
            {
                var orderdto = new OrderDto();
                var order = orderProductRepo.GetOrder(int.Parse(id));
                var prodLst = _mapper.Map<List<ProductDto>>(order);
                orderdto.OrderId = order.FirstOrDefault().OrderId;
                orderdto.Products = prodLst;
                orderdto.TotalQuantity = prodLst.Select(p => p.Quantity).Sum();
                orderdto.TotalPrice = prodLst.Select(p => p.Price).Sum();
                return Ok(orderdto);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost("add-Order/{id}")]
        public IActionResult AddOrder(string id, [FromBody] OrderDto prod)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return Unauthorized("Invalid Authentication");
                return Ok(orderProductRepo.AddOrderProducts(prod,id));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("edit-Order")]
        public IActionResult EditOrder([FromBody] OrderDto prod)
        {
            try
            {
                return Ok(orderProductRepo.EditOrderProduct(prod));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("delete-orders")]
        public IActionResult DeleteOrders([FromBody] List<OrderDto> orders)
        {
            try
            {

                return Ok(orderProductRepo.DeleteOrders(orders));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        #endregion

        #region Product

        [HttpGet("GetProducts")]
        public IActionResult GetProducts()
        {
            try
            {
                var userId = User.Claims?.FirstOrDefault(x => x.Type.ToLower() == "userId".ToLower())?.Value?.ToString();

                var prod = orderProductRepo.GetProducts();
                var prodLst = _mapper.Map<List<ProductDto>>(prod);
                return Ok(prodLst);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
        [HttpGet("GetProduct/{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var prod = orderProductRepo.GetProduct(id);
                var prodLst = _mapper.Map<ProductDto>(prod);
                return Ok(prodLst);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost("add-product")]
        public IActionResult AddProduct([FromBody] ProductDto prod)
        {
            try
            {

                var prodEntity = _mapper.Map<Product>(prod);
                return Ok(orderProductRepo.AddProduct(prodEntity));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("update-product")]
        public IActionResult UpdateProduct([FromBody] ProductDto prod)
        {
            try
            {
                var prodEntity = _mapper.Map<Product>(prod);

                return Ok(orderProductRepo.EditProduct(prodEntity));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("delete-products")]
        public IActionResult DeletProduct([FromBody] List<ProductDto> prods)
        {
            try
            {
                var prodEntity = _mapper.Map<List<Product>>(prods);

                return Ok(orderProductRepo.DeleteProduct(prodEntity));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        #endregion
    }
}
