using Pitstop.Models.PitstopData;
using Pitstop.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Pitstop.Service
{
    public class CommonService
    {
        private readonly PitstopContext _PitstopContext;
        public CommonService(PitstopContext PitstopContext)
        {
            _PitstopContext = PitstopContext;
        }

        public string GetDescriptionOfRole(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return string.Empty;
            }

            var userRolesList = _PitstopContext.UserRoles.Where(e => e.UserId == userId).ToList();
            var result = string.Empty;

            var i = 1;
            foreach (var item in userRolesList)
            {
                var role = _PitstopContext.Roles.Find(item.RoleId);

            
                if (i == userRolesList.Count())
                {
                    result += $"{role.Name}";
                }
                else
                {
                    result += $"{role.Name} / ";
                }

                i++;
            }

            return result;
        }
        public string GetUserName(string userId)
        {
            var result = _PitstopContext.Users.Find(userId);

            if (result == null)
            {
                return string.Empty;
            }

            return result.UserName;
        }

        //Not used cause can not fill the requirement of password
        public string NotUsedGetCharacterRandomString(int size)
        {
            Random random = new Random();
            var chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789~!@#$%^&*()";
            return new string(chars.Select(c => chars[random.Next(chars.Length)]).Take(size).ToArray());
        }

        //Shuffle the strings
        public string Scramble(string s)
        {
            try
            {
                return new string(s.ToCharArray().OrderBy(x => Guid.NewGuid()).ToArray());
            }
            catch
            {
                return s;
            }
        }

        public string GetCharacterRandomString(int size)
        {
            Random random = new Random();
            var result = string.Empty;

            size = Convert.ToInt32(size / 4);

            var charsSmall = "abcdefghijklmnopqrstuvwxyz";
            result = new string(charsSmall.Select(c => charsSmall[random.Next(charsSmall.Length)]).Take(size).ToArray());

            var charsCapital = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            result += new string(charsCapital.Select(c => charsCapital[random.Next(charsCapital.Length)]).Take(size).ToArray());

            var charsNumber = "0123456789";
            result += new string(charsNumber.Select(c => charsNumber[random.Next(charsNumber.Length)]).Take(size).ToArray());

            var charsSymbol = "~!@#$%^&*()";
            result += new string(charsSymbol.Select(c => charsSymbol[random.Next(charsSymbol.Length)]).Take(size).ToArray());

            return Scramble(result);
        }

        public DataControl GetDataControlData(string? id, string? parentType, string? type)
        {
            if (id != null)
            {
                return _PitstopContext.DataControls.Find(id);
            }

            if (parentType != null && type == null)
            {
                return _PitstopContext.DataControls.FirstOrDefault(e => e.IsActive == true && e.Description == parentType);
            }

            if (parentType != null && type != null)
            {
                var dataParent = _PitstopContext.DataControls.FirstOrDefault(e => e.IsActive == true && e.Description == parentType);
                return _PitstopContext.DataControls.FirstOrDefault(e => e.IsActive == true && e.Parent == dataParent.Id && e.Description == type);
            }

            return null;
        }

        public string GetIdOfSystemByName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var dataSystem = _PitstopContext.Systems.FirstOrDefault(e => e.Name.Contains(name));

                if (dataSystem != null)
                {
                    return dataSystem.Id;
                }

                return string.Empty;
            }

            return string.Empty;
        }

        public string GetNameOfSystemById(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var dataSystem = _PitstopContext.Systems.Find(id);

                if (dataSystem != null)
                {
                    return dataSystem.Name;
                }

                return string.Empty;
            }

            return string.Empty;

        }
        public string GetAccessName(string userId)
        {
            var systemMapping = _PitstopContext.UserSystemMappings.Where(e => e.UserId == userId).ToList();
            var result = string.Empty;

            var i = 1;
            foreach (var item in systemMapping)
            {
                var system = _PitstopContext.Systems.Find(item.SystemId);

                if (i == systemMapping.Count())
                {
                    result += $"{system.Name}";
                }
                else
                {
                    result += $"{system.Name} / ";
                }

                i++;
            }

            return result;
        }
        public class DynamicCommonModel
        {
            public int? IdInt { get; set; }
            public string IdString { get; set; }
            public string Value { get; set; }
        }
        public List<DynamicCommonModel> GetStatusList()
        {
            var list = new List<DynamicCommonModel>();

            list.Add(new DynamicCommonModel { IdInt = null, Value = "All" });
            list.Add(new DynamicCommonModel { IdInt = 1, Value = "Active" });
            list.Add(new DynamicCommonModel { IdInt = 0, Value = "Inactive" });

            return list;
        }

        public List<DynamicCommonModel> GetRoleList()
        {
            var list = new List<DynamicCommonModel>();

            var data = _PitstopContext.Roles.Where(e => e.Name != null).ToList();

            list.Add(new DynamicCommonModel { IdString = string.Empty, Value = "All" });

            foreach (var item in data)
            {
                list.Add(new DynamicCommonModel { IdString = item.Id, Value = item.Name });
            }

            return list;
        }

               public List<Carousel2> GetLatestCarousel2()
        {
            return _PitstopContext.Carousel2
                .OrderByDescending(h => h.CreatedAt)
                .Take(3)
                .ToList();
        }
    }
}
