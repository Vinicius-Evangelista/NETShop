using Discount.Grpc.Data;
using Discount.Grpc.Models;
using Grpc.Core;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Discount.Grpc.Services;

public class DiscountService(
    DiscountDbContext dbContext,
    ILogger<DiscountService> logger
) : DiscountProtoService.DiscountProtoServiceBase
{
    public override async Task<CouponModel> GetDiscount(
        GetDiscountRequest request,
        ServerCallContext context
    )
    {
        var coupon =
            await dbContext.Coupons.FirstOrDefaultAsync(
                predicate: c => c.ProductName == request.ProductName
            )
            ?? new Coupon
            {
                ProductName = "No Discount",
                Amount = 0,
                Description = "",
            };

        logger.LogInformation(
            message: "Discount is retrieved for product: {ProductName}",
            coupon.ProductName
        );

        var couponModel = coupon.Adapt<CouponModel>();

        return couponModel;
    }

    public override async Task<CouponModel> CreateDiscount(
        CreateDiscountRequest request,
        ServerCallContext context
    )
    {
        var coupon = request.Coupon.Adapt<Coupon>();
        if (coupon is null)
        {
            throw new RpcException(
                status: new Status(
                    statusCode: StatusCode.InvalidArgument,
                    detail: "Coupon cannot be null"
                )
            );
        }

        dbContext.Coupons.Add(entity: coupon);
        await dbContext.SaveChangesAsync();

        logger.LogInformation(
            message: "Discount is successfully created for product: {ProductName}",
            coupon.ProductName
        );

        return coupon.Adapt<CouponModel>();
    }

    public override async Task<CouponModel> UpdateDiscount(
        UpdateDiscountRequest request,
        ServerCallContext context
    )
    {
        var coupon = request.Coupon.Adapt<Coupon>();

        if (coupon is null)
        {
            throw new RpcException(
                status: new Status(
                    statusCode: StatusCode.InvalidArgument,
                    detail: "Coupon cannot be null"
                )
            );
        }

        dbContext.Coupons.Update(entity: coupon);
        await dbContext.SaveChangesAsync();

        logger.LogInformation(
            message: "Discount is successfully updated for product: {ProductName}",
            coupon.ProductName
        );

        var couponModel = coupon.Adapt<CouponModel>();
        return couponModel;
    }

    public override async Task<DeleteDiscountResponse> DeleteDiscount(
        DeleteDiscountRequest request,
        ServerCallContext context
    )
    {
        var coupon = await dbContext.Coupons.FirstOrDefaultAsync(
            predicate: c => c.ProductName == request.ProductName
        );

        if (coupon is null)
        {
            throw new RpcException(
                status: new Status(
                    statusCode: StatusCode.NotFound,
                    detail: "Coupon not found"
                )
            );
        }

        dbContext.Coupons.Remove(entity: coupon);
        await dbContext.SaveChangesAsync();

        logger.LogInformation(
            message: "Discount is successfully deleted for product: {ProductName}",
            coupon.ProductName
        );

        return new DeleteDiscountResponse { Success = true };
    }
}
