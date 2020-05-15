using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MoviesAPI.Dtos
{
    public class FilterMovieTheatersDto
    {
        [BindRequired]
        [Range(-90, 90)]
        public double Lat { get; set; }
        [BindRequired]
        [Range(-180, 180)]
        public double Long { get; set; }
        private int distanceInKms = 10;
        private int maxDistanceInKms = 50;
        public int DistanceInKms
        {
            get
            {
                return distanceInKms;
            }
            set
            {
                distanceInKms = (value > maxDistanceInKms) ? maxDistanceInKms : value;
            }
        }
    }
}