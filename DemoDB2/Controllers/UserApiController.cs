using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DemoDB2.Models;
using System.Data.Entity;
using System.IO;
using System.Web;

namespace DemoDB2.Controllers
{
    [RoutePrefix("api/user")]
    public class UserApiController : ApiController
    {
        private QLKSEntities database = new QLKSEntities();
        //get : api/user/GetAllUser
        [Route("GetAllUsers")]
        [HttpGet]
        public IHttpActionResult GetAllUsers()
        {
            try
            {
                var allUsers = database.NguoiDung
                    .Select(p => new
                    {
                        p.TenNguoiDung,
                        p.NguoiDungID,
                        p.SoDienThoai,
                        p.ImageUser,
                        p.Email,
                        p.DiaChi
                    })
                    .ToList();
                return Ok(allUsers);
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
