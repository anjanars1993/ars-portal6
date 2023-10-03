
using ars_portal6.Models;
using ars_portal6.Models.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.Linq;

namespace ars_portal.Controllers
{
    [Route("api/[controller]")]
    [Controller]
    public class EmployeesDetailedDataController : ODataController
    {
        private readonly DbContextEmployees context;
        public EmployeesDetailedDataController(DbContextEmployees _context)
        {
            this.context = _context;
        }
        [EnableQuery()]
        [HttpGet]
        public IQueryable<EmployeesDetailedData> Get()
        {
            return context.EmployeesDetailedData;
        }
        [EnableQuery()]
        [HttpGet("{id}")]
        public SingleResult<EmployeesDetailedData> Get([FromODataUri] int id)
        {
            //return context.EmployeesDetailedData.FirstOrDefault(s => s.id == key);
            return SingleResult.Create(context.EmployeesDetailedData.Where(c => c.id == id));
        }
        [HttpPost]
        public void Post([FromBody] EmployeesDetailedData value)
        {
            context.EmployeesDetailedData.Add(value);
            context.SaveChanges();
        }
        [HttpPut]
        [Route("{id:int}")]
        public void Put(int id, [FromBody] EmployeesDetailedData value)
        {
            var skill = context.Skill.Where(x => x.employeesDetailedDataId == value.id).ToList();
            foreach (var skillItem in skill)
            {
                context.Skill.Remove(skillItem);
                context.SaveChanges();
            }
            foreach (var skillItem in value.skills)
            {
                skillItem.employeesDetailedDataId = value.id;
                context.Skill.Add(skillItem);
                context.SaveChanges();
            }
            context.EmployeesDetailedData.Update(value);
            context.SaveChanges();
        }
        [HttpDelete]
        [Route("{id:int}")]
        public void Delete(int id)
        {
            using (IDbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var employee = context.EmployeesDetailedData.FirstOrDefault(s => s.id == id);
                    var skill = context.Skill.Where(x => x.employeesDetailedDataId == employee.id).ToList();
                    foreach (var skillItem in skill)
                    {
                        context.Skill.Remove(skillItem);
                        context.SaveChanges();
                    }
                    if (employee != null)
                    {
                        context.EmployeesDetailedData.Remove(employee);
                        context.SaveChanges();
                    }
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

        }
    }
}
