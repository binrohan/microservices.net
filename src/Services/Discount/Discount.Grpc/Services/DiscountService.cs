﻿using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories;
using Grpc.Core;

namespace Discount.Grpc.Services;

public class DiscountService(IDiscountRepository repo,
                             ILogger<DiscountService> logger,
                             IMapper mapper) : DiscountProtoService.DiscountProtoServiceBase
{
    private readonly IDiscountRepository _repo = repo;
    private readonly ILogger<DiscountService> _logger = logger;
    private readonly IMapper _mapper = mapper;

    public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
    {
        var coupon = await _repo.Get(request.ProductName) 
            ?? throw new RpcException(new Status(StatusCode.NotFound, $"Discount with ProductName={request.ProductName} is not found."));

        _logger.LogInformation("Discount is retrieved for ProductName: {productName}, Amount: {amount}", coupon.ProductName, coupon.Amount);

        var couponModel = _mapper.Map<CouponModel>(coupon);

        return couponModel!;
    }

    public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
    {
        var coupon = _mapper.Map<Coupon>(request.Coupon);

        await _repo.Create(coupon);

        _logger.LogInformation("Discount is successfully created. ProductName : {ProductName}", coupon.ProductName);

        var couponModel = _mapper.Map<CouponModel>(coupon);

        return couponModel;
    }

    public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
    {
        var coupon = _mapper.Map<Coupon>(request.Coupon);

        await _repo.Update(coupon);

        _logger.LogInformation("Discount is successfully updated. ProductName : {ProductName}", coupon.ProductName);

        var couponModel = _mapper.Map<CouponModel>(coupon);

        return couponModel;
    }

    public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
    {
        var deleted = await _repo.Delete(request.ProductName);

        return new DeleteDiscountResponse
        {
            Success = deleted
        };
    }
}
