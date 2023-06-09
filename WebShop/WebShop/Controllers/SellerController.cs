﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebShop.DTO;
using WebShop.Exceptions;
using WebShop.Interfaces;
using WebShop.Services;

namespace WebShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SellerController : ControllerBase
    {
        private readonly ISellerService _sellerService;
        public SellerController(ISellerService sellerService)
        {
            _sellerService = sellerService;
        }

        [HttpGet("products/{id}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> GetProduct(int id)
        {
            if (!int.TryParse((User.Claims.First(c => c.Type == "UserId").Value), out int sellerId))
                throw new BadRequestException("Error occured with ID. Please try again.");

            var product = await _sellerService.GetProduct(id, sellerId);
            return Ok(product);
        }

        [HttpGet("products")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> GetAllProducts()
        {
            if (!int.TryParse((User.Claims.First(c => c.Type == "UserId").Value), out int sellerId))
                throw new BadRequestException("Error occured with ID. Please try again.");

            var products = await _sellerService.GetAllProducts(sellerId);
            return Ok(products);
        }

        [HttpGet("orders")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> GetAllOrders()
        {
            if (!int.TryParse((User.Claims.First(c => c.Type == "UserId").Value), out int sellerId))
                throw new BadRequestException("Error occured with ID. Please try again.");

            var orders = await _sellerService.GetAllOrders(sellerId);
            return Ok(orders);
        }

        [HttpGet("new-orders")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> GetNewOrders()
        {
            if (!int.TryParse((User.Claims.First(c => c.Type == "UserId").Value), out int sellerId))
                throw new BadRequestException("Error occured with ID. Please try again.");

            var orders = await _sellerService.GetNewOrders(sellerId);
            return Ok(orders);
        }

        [HttpPost("products")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> CreateNewProduct([FromForm]CreateProductDTO productDTO)
        {
            if (!int.TryParse((User.Claims.First(c => c.Type == "UserId").Value), out int sellerId))
                throw new BadRequestException("Error occured with ID. Please try again.");

            await _sellerService.CreateNewProduct(productDTO, sellerId);
            return Ok();
        }

        [HttpDelete("products/{id}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (!int.TryParse((User.Claims.First(c => c.Type == "UserId").Value), out int sellerId))
                throw new BadRequestException("Error occured with ID. Please try again.");

            await _sellerService.DeleteProduct(id, sellerId);
            return Ok();
        }

        [HttpPut("products/{id}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm]CreateProductDTO productDTO)
        {
            if (!int.TryParse((User.Claims.First(c => c.Type == "UserId").Value), out int sellerId))
                throw new BadRequestException("Error occured with ID. Please try again.");

            await _sellerService.UpdateProduct(id, productDTO, sellerId);
            return Ok();
        }

        [HttpPost("accept-order/{id}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> AcceptOrder(int id)
        {
            if (!int.TryParse((User.Claims.First(c => c.Type == "UserId").Value), out int sellerId))
                throw new BadRequestException("Error occured with ID. Please try again.");

            await _sellerService.AcceptOrder(id, sellerId);
            return Ok();
        }
    }
}
