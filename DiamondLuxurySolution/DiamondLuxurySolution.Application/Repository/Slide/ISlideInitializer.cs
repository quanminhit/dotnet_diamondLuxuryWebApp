﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondLuxurySolution.Application.Repository.Slide
{
	public interface ISlideInitializer
	{
		Task CreateDefaultSlide();
	}
}
