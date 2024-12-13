using Application.Users.Command.SetNameAndAvatar;
using Application.Users.Command.SignUp;
using Application.Users.Querry.Login;
using AutoMapper;
using Domain.Entities;
using PlanGuruAPI.DTOs.GroupDTOs;
using PlanGuruAPI.DTOs.UserDTOs;

namespace PlanGuruAPI.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<LoginRequest, LoginQuerry>().ReverseMap();
            CreateMap<SignUpRequest, SignUpCommand>().ReverseMap();
            CreateMap<SetNameAndAvatarRequest, SetNameAndAvatarCommand>().ReverseMap();
            CreateMap<CreateGroupRequest, Group>().ReverseMap();
            CreateMap<JoinGroupRequest, GroupUser>().ReverseMap();
            CreateMap<CreatePostInGroupRequest, Post>().ReverseMap();
            CreateMap<Post, DTOs.PlantPostDTOs.PostReadDTO>().ReverseMap();
            CreateMap<PostInGroupDTO, Post>().ReverseMap()
                .ForMember(dest => dest.NumberOfComment, opt => opt.MapFrom(p => p.PostComments.Count))
                .ForMember(dest => dest.NumberOfUpvote, opt => opt.MapFrom(p => p.PostUpvotes.Count))
                .ForMember(dest => dest.NumberOfDevote, opt => opt.MapFrom(p => p.PostDevotes.Count))
                .ForMember(dest => dest.NumberOfShare, opt => opt.MapFrom(p => p.PostShares.Count))
                .ForMember(dest => dest.UserNickName, opt => opt.MapFrom(p => p.User.Name))
                .ForMember(dest => dest.UserAvatar, opt => opt.MapFrom(p => p.User.Avatar))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(p => FormatCreatedAt(p.CreatedAt)));

            
        }

        public static string FormatCreatedAt(DateTime createdAt)
        {
            var timeSpan = DateTime.UtcNow - createdAt;
            if (timeSpan.TotalMinutes < 1)
            {
                return "just now";
            }
            else if (timeSpan.TotalHours < 1)
            {
                int minutes = (int)timeSpan.TotalMinutes;
                return minutes == 1 ? "1 minute ago" : $"{minutes} minutes ago";
            }
            else if (timeSpan.TotalHours < 24)
            {
                int hours = (int)timeSpan.TotalHours;
                return hours == 1 ? "1 hour ago" : $"{hours} hours ago";
            }
            else if (timeSpan.TotalDays < 7)
            {
                int days = (int)timeSpan.TotalDays;
                return days == 1 ? "1 day ago" : $"{days} days ago";
            }
            else
            {
                return createdAt.ToString("dd-MM-yyyy HH:mm");
            }
        }
    }
}
