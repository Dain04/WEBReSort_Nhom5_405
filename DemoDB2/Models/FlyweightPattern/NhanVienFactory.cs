using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using DemoDB2.Models;

public class NhanVienFactory
{
    private static readonly ObjectCache _cache = MemoryCache.Default;

    public NhanVien GetCustomer(int id)
    {
        string cacheKey = "Customer_" + id;

        if (!_cache.Contains(cacheKey))
        {
            Console.WriteLine($"Tạo mới dữ liệu khách hàng {id}");
            var customer = new NhanVien(id); // Truyền id vào constructor

            CacheItemPolicy policy = new CacheItemPolicy
            {
                SlidingExpiration = TimeSpan.FromMinutes(10) // Cache tồn tại 10 phút
            };

            _cache.Add(cacheKey, customer, policy);
        }

        return (NhanVien)_cache.Get(cacheKey);
    }

}
