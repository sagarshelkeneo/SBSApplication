using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Product.Dtos;
using Client.Application.Interfaces;
using Client.Domain.Models;
using Client.Persistence.Context;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Client.Persistence.Repositories
{
    public class ProductRepository : IProductRepository
    {
        //private readonly AppDbContext _context;
        private readonly IDbConnection _db;

        //public ProductRepository(AppDbContext context,IDbConnection dbConnection)
        public ProductRepository(IDbConnection dbConnection)
        {
            //_context = context;
            _db = dbConnection;

        }

       
        public async Task<List<ProductDto>> CreateProductAsync(CreateProductDto dto)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@P_description", dto.Description);
            parameters.Add("@P_unitPrice", dto.UnitPrice);
            parameters.Add("@P_companyID", dto.CompanyId);
            parameters.Add("@P_createdBy", dto.CreatedBy);

            var result = await _db.QueryFirstOrDefaultAsync<InsertProductResult>(
                "usp_sbs_productMaster_insert",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (result.R_Status == "SUCCESS" && result.R_InsertedID.HasValue)
            {
                //return new ProductDto
                //{
                //    R_id = result.R_InsertedID.Value,
                //    R_description = dto.Description,
                //    R_unitPrice = dto.UnitPrice,

                //};
                return await GetProductsAsync(dto.CompanyId, null, null);
            }

            throw new Exception($"Insert Failed: {result.R_ErrorMessage} (ErrorCode: {result.R_ErrorNumber})");
        }
       
        public async Task<List<ProductDto>> UpdateProductAsync(UpdateProductDto dto)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@P_id", dto.Id);
            parameters.Add("@P_description", dto.Description);
            parameters.Add("@P_unitPrice", dto.UnitPrice);
            parameters.Add("@P_companyID", dto.CompanyId);
            parameters.Add("@P_updatedBy", dto.UpdatedBy);

            var result = await _db.QueryFirstOrDefaultAsync<string>(
                "usp_sbs_productMaster_update",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (result == "SUCCESS")
            {
                var product = await _db.QueryFirstOrDefaultAsync<Product>(
                    @"SELECT id, description, unitPrice 
              FROM sbs_productMaster 
              WHERE id = @Id AND isDeleted = 0",
                    new { Id = dto.Id }
                );

                if (product == null)
                    throw new Exception("Updated product not found");

                return await GetProductsAsync(dto.CompanyId, null, null);

            }

            throw new Exception("Product update failed");
        }
        public async Task<List<ProductDto>> DeleteProductAsync(int id, int updatedBy, int companyId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@P_id", id);
            parameters.Add("@P_updatedBy", updatedBy);

            var result = await _db.QueryFirstOrDefaultAsync<dynamic>(
                "usp_sbs_productMaster_delete",
                parameters,
                commandType: CommandType.StoredProcedure
            );
            if (result == null || result.R_Status != "SUCCESS")
                throw new Exception($"Update failed: {result?.R_ErrorMessage ?? "Unknown error"}");

            return await GetProductsAsync(companyId,null, null);
        }

        public async Task<List<ProductDto>> GetProductsAsync(int companyId,int? id, string? search)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_id", id);
            parameters.Add("@P_Search", search);
            parameters.Add("@P_companyID", companyId);

            var result = await _db.QueryAsync<ProductDto>(
                "usp_sbs_productMaster_get",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }

    }
}
