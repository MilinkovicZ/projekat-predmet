﻿using AutoMapper;
using AutoMapper.Configuration.Annotations;
using MailKit.Search;
using Microsoft.EntityFrameworkCore;
using WebShop.DTO;
using WebShop.Enums;
using WebShop.Exceptions;
using WebShop.Interfaces;
using WebShop.Models;
using static System.Net.Mime.MediaTypeNames;

namespace WebShop.Services
{
    public class SellerService : ISellerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public SellerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task CreateNewProduct(CreateProductDTO productDTO, int sellerId)
        {
            User? seller = await _unitOfWork.UsersRepository.Get(sellerId);
            if (seller == null)
                throw new UnauthorizedException($"Unable to find user with ID: {sellerId}.");

            var product = _mapper.Map<Product>(productDTO);
            product.SellerId = sellerId;

            if(productDTO.ImageForm != null)
            {
                using (var ms = new MemoryStream())
                {
                    productDTO.ImageForm.CopyTo(ms);
                    var imageBytes = ms.ToArray();

                    product.Image = imageBytes;
                }
            }

            await _unitOfWork.ProductsRepository.Insert(product);
            await _unitOfWork.Save();
        }

        public async Task DeleteProduct(int productId, int sellerId)
        {
            User? seller = await _unitOfWork.UsersRepository.Get(sellerId);
            if (seller == null)
                throw new UnauthorizedException($"Unable to find user with ID: {sellerId}.");

            Product? product = await _unitOfWork.ProductsRepository.Get(productId);
            if (product == null || product.SellerId != sellerId)
                throw new NotFoundException($"Unable to find product with ID: {productId}.");

            _unitOfWork.ProductsRepository.Delete(product);
            await _unitOfWork.Save();
        }

        public async Task UpdateProduct(int productId, CreateProductDTO productDTO, int sellerId)
        {
            User? seller = await _unitOfWork.UsersRepository.Get(sellerId);
            if (seller == null)
                throw new UnauthorizedException($"Unable to find user with ID: {sellerId}.");

            Product? product = await _unitOfWork.ProductsRepository.Get(productId);
            if (product == null || product.SellerId != sellerId)
                throw new NotFoundException($"Unable to find product with ID: {productId}.");

            _mapper.Map(productDTO, product);

            if (productDTO.ImageForm != null)
            {
                using (var ms = new MemoryStream())
                {
                    productDTO.ImageForm.CopyTo(ms);
                    var imageBytes = ms.ToArray();

                    product.Image = imageBytes;
                }
            }

            _unitOfWork.ProductsRepository.Update(product);
            await _unitOfWork.Save();
        }

        public async Task<List<OrderDTO>> GetAllOrders(int sellerId)
        {
            User? seller = await _unitOfWork.UsersRepository.Get(sellerId);
            if (seller == null)
                throw new UnauthorizedException($"Unable to find user with ID: {sellerId}.");

            var products = await _unitOfWork.ProductsRepository.GetAll();
            var filteredProduct = products.Where(p => p.SellerId == sellerId);

            var productsIds = filteredProduct.Select(x => x.Id);

            var orders = await _unitOfWork.OrdersRepository.GetAll();
            var includedOrders = orders.Where(o => o.OrderState == OrderState.Delievered).Include(o => o.Items);
            var sellerOrders = includedOrders.Where(o => o.Items.Any(i => productsIds.Contains(i.ProductId))).ToList();

            foreach (var order in sellerOrders)
            {
                order.Items = order.Items!.FindAll(i => productsIds.Contains(i.ProductId));
            }

            return _mapper.Map<List<OrderDTO>>(sellerOrders);
        }

        public async Task<List<OrderDTO>> GetNewOrders(int sellerId)
        {
            User? seller = await _unitOfWork.UsersRepository.Get(sellerId);
            if (seller == null)
                throw new UnauthorizedException($"Unable to find user with ID: {sellerId}.");

            var products = await _unitOfWork.ProductsRepository.GetAll();
            var filteredProduct = products.Where(p => p.SellerId == sellerId);

            var productsIds = filteredProduct.Select(x => x.Id);

            var orders = await _unitOfWork.OrdersRepository.GetAll();
            var includedOrders = orders.Where(o => o.OrderState == OrderState.Preparing).Include(o => o.Items);
            var sellerOrders = includedOrders.Where(o => o.Items.Any(i => productsIds.Contains(i.ProductId))).ToList();

            foreach (var order in sellerOrders)
            {
                order.Items = order.Items!.FindAll(i => productsIds.Contains(i.ProductId));
            }

            return _mapper.Map<List<OrderDTO>>(sellerOrders);
        }

        public async Task<ProductDTO> GetProduct(int productId, int sellerId)
        {
            User? seller = await _unitOfWork.UsersRepository.Get(sellerId);
            if (seller == null)
                throw new UnauthorizedException($"Unable to find user with ID: {sellerId}.");

            Product? product = await _unitOfWork.ProductsRepository.Get(productId);
            if (product == null)
                throw new NotFoundException($"Unable to find product with ID: {productId}.");

            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<List<ProductDTO>> GetAllProducts(int sellerId)
        {
            User? seller = await _unitOfWork.UsersRepository.Get(sellerId);
            if (seller == null)
                throw new UnauthorizedException($"Unable to find user with ID: {sellerId}.");

            var products = await _unitOfWork.ProductsRepository.GetAll();
            List<Product> sellerProducts = products.Where(x => x.SellerId == sellerId).ToList();
            return _mapper.Map<List<ProductDTO>>(sellerProducts);
        }

        public async Task AcceptOrder(int orderId, int sellerId)
        {
            User? seller = await _unitOfWork.UsersRepository.Get(sellerId);
            if (seller == null)
                throw new UnauthorizedException($"Unable to find user with ID: {sellerId}.");

            var products = await _unitOfWork.ProductsRepository.GetAll();
            var filteredProduct = products.Where(p => p.SellerId == sellerId);
            var productsIds = filteredProduct.Select(x => x.Id);

            var orders = await _unitOfWork.OrdersRepository.GetAll();
            var includedOrders = orders.Where(o => o.isAccepted == false).Include(o => o.Items);
            var order = includedOrders.FirstOrDefault(o => o.Id == orderId);
            if (order == null)
                throw new BadRequestException($"Unable to find unaccepted order with ID: {orderId}.");
            if (!order.Items.Any(i => productsIds.Contains(i.ProductId)))
                throw new BadRequestException($"You dont have any products on order with ID: {orderId}.");

            order.isAccepted = true;
            order.StartTime = DateTime.Now;
            order.DeliveryTime = DateTime.Now.AddHours(1).AddMinutes(new Random().Next(59));
            _unitOfWork.OrdersRepository.Update(order);
            await _unitOfWork.Save();
        }
    }
}
