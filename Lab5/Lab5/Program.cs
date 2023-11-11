﻿using System.IO;
using System.Linq;

await File.WriteAllBytesAsync(args[1],
								(await File.ReadAllBytesAsync(args[0])).Select((value, index) => {

									if(index % 4 != 0) {

										return value;

									}

									byte result = value;
									int size = 7;

									for(value >>= 1; value != 0; value >>= 1) {

										result <<= 1;
										result |= (byte) (value & 1);

										size--;

									}

									result <<= size;

									return result;

								})
								.ToArray());
