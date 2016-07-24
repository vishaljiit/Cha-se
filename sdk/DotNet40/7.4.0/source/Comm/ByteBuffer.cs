
using System;

// Disables warnings for XML doc comments.
#pragma warning disable 1591
#pragma warning disable 1573
#pragma warning disable 1572
#pragma warning disable 1571
#pragma warning disable 1587
#pragma warning disable 1570

namespace Paymentech.Comm
{

	// Title: Byte Buffer 
	//
	// Description: abbreviated version of java's ByteBuffer
	//
	// Copyright (c) 2010, Chase Paymentech Solutions, LLC. All rights
	// reserved
	//
	// Company: Chase Paymentech Solutions
	//
	/// @author Rameshkumar Bhaskharan
	// @version 3.0
	// 

	/// <summary>
	/// A class that encapsulates access to a byte array with 
	/// behavior similar to the Java ByteBuffer class
	/// </summary>
	public class ByteBuffer
	{
		byte[] bytes = null;
		ulong position = 0;
		ulong limit = 0;
		ulong capacity = 0;
		const int numIntBytes = 4;

		//Properties
		/// <summary>
		/// 
		/// </summary>
		public byte [] Bytes
		{
			get
			{
				return bytes;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public ulong Position
		{
			get
			{
				return position;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public ulong Limit
		{
			get
			{
				return limit;
			}
		}

		// Constructors
		/// <summary>
		/// 
		/// </summary>
		/// <param name="size"></param>
		public ByteBuffer( ulong size )
		{
			bytes = new byte[ size ];
			capacity = limit = size;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="size"></param>
		public ByteBuffer( uint size ) : this( (ulong) size ) {}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="size"></param>
		public ByteBuffer( int size ) : this( (ulong) size ) {}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public static ByteBuffer Allocate( int size )
		{
			return new ByteBuffer( size );
		}

		// 

		/// <summary>
		/// this is used for creating a ByteBuffer that is
		/// backed by the buffer specified ( eliminates need to
		/// copy bytes )
		/// </summary>
		/// <param name="buff">actual byte ar</param>
		/// <param name="offset"></param>
		/// <param name="length"></param>
		public ByteBuffer( byte[] buff, ulong offset, ulong length )
		{
			if ( ( ( ulong ) buff.Length ) < ( offset + length ) )
			{
				throw new Exception( "Length of ByteBuffer array must not be smaller than offset + length" );
			}

			// when we get the special case that offset is not
			// zero, we copy the bytes into a new buffer.
			// This is different than java ByteBuffer but
			// is a rare situation and makes things a whole lot
			// simpler
			if ( offset != 0 )
			{
				byte [] newbuf = new byte[ length ];
				for ( ulong i=0,j=offset; ( i < length ) 
					&& ( j < (ulong) buff.Length ) ; i++, j++ )
				{
					newbuf[ i ] = buff[ j ];
				}
				bytes = newbuf;
			}
			else // just back the buffer with what we were given
			{
				bytes = buff;
			}

			capacity = limit = length;
		}

		/// <summary>
		/// convenience versions of the Wrap() method allowing for no offset or length
		/// and integer arguments
		/// </summary>
		/// <param name="buff"> source bytes </param>
		/// <param name="offset"> offset into the source byte array to start copying</param>
		/// <param name="length"> number of bytes to copy from source byte array</param>
		/// <returns></returns>
		public static ByteBuffer Wrap( byte[] buff, int offset, int length )
		{
			return new ByteBuffer( buff, (ulong) offset, (ulong) length );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="buff"></param>
		/// <returns></returns>
		public static ByteBuffer Wrap( byte[] buff )
		{
			return new ByteBuffer( buff, (ulong) 0, (ulong) buff.Length );
		}

		/// <summary>
		/// Used for overwriting the bytes inside a byte buffer
		/// </summary>
		/// <param name="buff"></param>
		/// <param name="offset"></param>
		/// <param name="length"></param>
		private void BlindCopy( byte[] buff, int offset, int length )
		{
			BlindCopy( buff, ( ulong ) offset, ( ulong ) length );
		}

		private void BlindCopy( byte[] buff, ulong offset, ulong length )
		{
			ulong end = offset + length;

			if ( end > ( ( ulong ) buff.Length ) )
			{
				throw new Exception( "Source buffer is length: " + buff.Length
					+ " but copy specfied to index: " + end );
			}

			for ( ulong i=0; ( i < ( ( ulong ) bytes.Length ) ) && ( i < length ); i++ )
			{
				bytes[ i ] = buff[ i + offset ];
			}
		}

		// many of these could be properties but we will keep them as methods to keep things
		// closer to how java is.  This will make C# and java versions (and the code that uses
		// them more interchangeable 
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public byte [] Array()
		{
			return bytes;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="buff"></param>
		public void Put( ByteBuffer buff )
		{
			byte[] guts = buff.Array();
			Put( guts, buff.Position, buff.Remaining() );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="single"></param>
		public void Put( byte single )
		{
			byte [] holder = new byte[ 1 ];
			holder[ 0 ] = single;
			Put( holder, 0, 1 );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public byte Get()
		{
			return bytes[ position++ ];
		}

		/// <summary>
		/// Main method for copying bytes into the ByteBuffer
		/// </summary>
		/// <param name="buff"></param>
		/// <param name="offset"></param>
		/// <param name="length"></param>
		public void Put( byte[] buff, ulong offset, ulong length )
		{
			for ( ulong j=0; ( position < limit ) && ( ( j + offset ) 
				< ( ( ulong ) buff.Length ) ); position++,j++ )
			{
				bytes[ position ] = buff[ offset + j ];
			}
		}

		// Convenience versions supporting different types of parameters
		/// <summary>
		/// 
		/// </summary>
		/// <param name="buff"></param>
		/// <param name="offset"></param>
		/// <param name="length"></param>
		public void Put( byte[] buff, int offset, int length )
		{
			Put( buff, ( ulong ) offset, ( ulong ) length );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="buff"></param>
		public void Put( byte[] buff )
		{
			Put( buff, ( ulong ) 0, ( ulong ) buff.Length );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="str"></param>
		public void Put( string str )
		{
			Put( System.Text.ASCIIEncoding.ASCII.GetBytes( str ), 
				( ulong ) 0, ( ulong ) str.Length ); 
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="intVal"></param>
		public void PutInt( int intVal )
		{
			Put( BitConverter.GetBytes( intVal ) );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="inc"></param>
		public void AddPosition( ulong inc )
		{
			position += inc;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public ulong Remaining()
		{
			return limit - position;
		}

		/// <summary>
		/// 
		/// </summary>
		public void Flip()
		{
			limit = position;
			position = 0;
		}

		/// <summary>
		/// Gets properly sized byte array of data from a ByteBuffer
		/// </summary>
		/// <returns></returns>
		public byte [] ExtractBytes()
		{
			byte [] retval = null;

			if ( Position > 0 )
			{
				byte [] src = bytes;
				retval = new byte[ Position ];

				for ( int i=0; i < ( int ) Position; i++ )
				{
					retval[ i ] = src[ i ];
				}
			}
			return retval;
		}

		override public string ToString()
		{
			string retval = null;

			if ( bytes != null )
			{
				retval = ByteArrayToString( bytes );
			}
			return retval;
		}

		public static string ByteArrayToString(byte[] bytes)
		{
			System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
			return enc.GetString(bytes);
		}
	}
}
