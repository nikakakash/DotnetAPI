// using Microsoft.AspNetCore.Mvc;
// using DotnetAPI.Dtos;
// using DotnetAPI.Models;
// using DotnetAPI.Data;
// using AutoMapper;
// namespace DotnetAPI.Controllers;



// [ApiController]
// [Route("[controller]")]
// public class UserEFController : ControllerBase
// {

//     IMapper _mapper;

//     IUserRepository _userRepository;

//     public UserEFController(IConfiguration config, IUserRepository userRepository)
//     {
//         _userRepository = userRepository;
//         _mapper = new Mapper(new MapperConfiguration(cfg => {
//             cfg.CreateMap<UserToAddDTO, User>();
//             cfg.CreateMap<UserSalary, UserSalary>();
//             cfg.CreateMap<UserJobInfo, UserJobInfo>();
//         }));
//     }


//     [HttpGet("GetUsers/")]
//     public IEnumerable<User> GetUser()
//     {
//         IEnumerable<User> users = _userRepository.GetUser();
//         return users;
//     }

//     [HttpGet("GetSingleUsers/{userId}")]
//     public User GetSingleUsers(int userId)
//     {
//         return _userRepository.GetSingleUser(userId);
//     }

//     [HttpPut("EditUser")]
//     public IActionResult EditUser(User user)
//     {
//         User? userDb = _userRepository.GetSingleUser(user.UserId);
//         if (userDb != null)
//         {
//             userDb.FirstName = user.FirstName;
//             userDb.LastName = user.LastName;
//             userDb.Email = user.Email;
//             userDb.Active = user.Active;
//             userDb.Gender = user.Gender;
//             if(_userRepository.SaveChanges())
//             {
//                 return Ok();
//             }
//             throw new Exception("Failed to update user");
//         }

//         throw new Exception("Failed to Get user");
//     }

//     [HttpPost("AddUser/")]
//     public IActionResult AddUser(UserToAddDTO user)
//     {
//         User userDb = _mapper.Map<User>(user); 
        
//         _userRepository.AddEntity<User>(userDb);

//         if(_userRepository.SaveChanges())
//         {
//             return Ok();
//         }
//         throw new Exception("Failed to Add user");
        
//     }


//     [HttpDelete("DeleteUser/{userId}")]
//     public IActionResult DeleteUser(int userId)
//     {
//         User? userDb = _userRepository.GetSingleUser(userId);
//         if (userDb != null)
//         {
//             _userRepository.RemoveEntity<User>(userDb);
//             if(_userRepository.SaveChanges())
//             {
//                 return Ok();
//             }
//             throw new Exception("Failed to Delete user");
//         }

//         throw new Exception("Failed to Get user");
//     }


    
// [HttpGet("UserSalary/{userId}")]
//     public UserSalary GetUser(int userId)
//     {
//         return _userRepository.GetSingleUserSalary(userId); 
//     }






// [HttpPost("UserSalary/")]
//     public IActionResult PostUserSalary(UserSalary userForInsert)
//     {
        
//         _userRepository.AddEntity<UserSalary>(userForInsert);

//         if(_userRepository.SaveChanges())
//         {
//             return Ok();
//         }
//         throw new Exception("Adding user salary failed");
        
//     }




//  [HttpPut("UserSalary")]
//     public IActionResult PutUserSalary(UserSalary UserforUpdate)
//     {
//         UserSalary? UsertotUpdate = _userRepository.GetSingleUserSalary(UserforUpdate.UserId);

//         if (UserforUpdate != null)
//         {
//             _mapper.Map(UserforUpdate, UserforUpdate);
//             if(_userRepository.SaveChanges())
//             {
//                 return Ok();
//             }
//             throw new Exception("Failed to update user salary");
//         }

//         throw new Exception("Failed to find user salary");
//     }




//     [HttpDelete("UserSalary/{userId}")]
//     public IActionResult DeleteUserSalary(int userId)
//     {
//         UserSalary? userToDelete = _userRepository.GetSingleUserSalary(userId);
//         if (userToDelete != null)
//         {
//            _userRepository.RemoveEntity<UserSalary>(userToDelete);
//             if(_userRepository.SaveChanges())
//             {
//                 return Ok();
//             }
//             throw new Exception("Failed to Delete user");
//         }

//         throw new Exception("Failed to Get user");
//     }





// [HttpGet("UserHobInfo/{userId}")]
//     public UserJobInfo GetUserJobInfo(int userId)
//     {
//         return  _userRepository.GetSingleUsersJobInfo(userId);
//     }





// [HttpPost("UserJobInfo/")]
//     public IActionResult PostUserJobInfo(UserJobInfo userForInsert)
//     {
        
//         _userRepository.AddEntity<UserJobInfo>(userForInsert);

//         if(_userRepository.SaveChanges())
//         {
//             return Ok();
//         }
//         throw new Exception("Adding user salary failed");
        
//     }




//  [HttpPut("UserJobInfo")]
//     public IActionResult PutUserJobInfo(UserJobInfo UserforUpdate)
//     {
//         UserJobInfo? UsertotUpdate = _userRepository.GetSingleUsersJobInfo(UserforUpdate.UserId);

//         if (UserforUpdate != null)
//         {
//             _mapper.Map(UserforUpdate, UserforUpdate);
//             if(_userRepository.SaveChanges())
//             {
//                 return Ok();
//             }
//             throw new Exception("Failed to update user salary");
//         }

//         throw new Exception("Failed to find user salary");
//     }




// [HttpDelete("UserJobInfo/{userId}")]
//     public IActionResult DeleteUserJobInfo(int userId)
//     {
//         UserJobInfo? userToDelete = _userRepository.GetSingleUsersJobInfo(userId);
//         if (userToDelete != null)
//         {
//            _userRepository.RemoveEntity<UserJobInfo>(userToDelete);
//             if(_userRepository.SaveChanges())
//             {
//                 return Ok();
//             }
//             throw new Exception("Failed to Delete user");
//         }

//         throw new Exception("Failed to Get user");
//     }



// }


 




   