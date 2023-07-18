/**************************************************************************
/* LongOpt.cs -- C#.NET port of Long option object for Getopt
/*
/* Copyright (c) 1998 by Aaron M. Renn (arenn@urbanophile.com)
/* C#.NET Port Copyright (c) 2004 by Klaus Prückl (klaus.prueckl@aon.at)
/*
/* This program is free software; you can redistribute it and/or modify
/* it under the terms of the GNU Library General Public License as published 
/* by  the Free Software Foundation; either version 2 of the License or
/* (at your option) any later version.
/*
/* This program is distributed in the hope that it will be useful, but
/* WITHOUT ANY WARRANTY; without even the implied warranty of
/* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/* GNU Library General Public License for more details.
/*
/* You should have received a copy of the GNU Library General Public License
/* along with this program; see the file COPYING.LIB.  If not, write to 
/* the Free Software Foundation Inc., 59 Temple Place - Suite 330, 
/* Boston, MA  02111-1307 USA
/**************************************************************************/

namespace GNU.Getopt
{
    /// <summary>
    /// Constant enumeration values used for the LongOpt <c>hasArg</c>
    /// constructor argument.
    /// </summary>
    public enum Argument 
	{
		/// <summary>
		/// This value indicates that the option takes no argument.
		/// </summary>
		No			= 0,
		/// <summary>
		/// This value indicates that the option takes an argument that is
		/// required.
		/// </summary>
		Required	= 1,
		/// <summary>
		/// This value indicates that the option takes an argument that is
		/// optional.
		/// </summary>
		Optional	= 2
	}
}