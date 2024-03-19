using AutoMapper;
using PostService.Dtos;
using PostService.Models;

namespace PostService.Profiles
{
    public class PostsProfile : Profile
    {
        public PostsProfile()
        {
            CreateMap<Post, PostReadDTO>();
            CreateMap<PostCreateDTO, Post>();
        }
    }
}