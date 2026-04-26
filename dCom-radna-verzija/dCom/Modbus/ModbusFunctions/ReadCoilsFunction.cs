using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read coil functions/requests.
    /// </summary>
    public class ReadCoilsFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadCoilsFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
		public ReadCoilsFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc/>
        public override byte[] PackRequest()
        {
            ModbusReadCommandParameters p = (ModbusReadCommandParameters)CommandParameters;

            byte[] request = new byte[12];

            // MBAP HEADER
            request[0] = (byte)(p.TransactionId >> 8);
            request[1] = (byte)(p.TransactionId & 0xFF);

            request[2] = 0x00;
            request[3] = 0x00;

            request[4] = 0x00;
            request[5] = 0x06;

            request[6] = p.UnitId;

            // PDU
            request[7] = p.FunctionCode;

            request[8] = (byte)(p.StartAddress >> 8);
            request[9] = (byte)(p.StartAddress & 0xFF);

            request[10] = (byte)(p.Quantity >> 8);
            request[11] = (byte)(p.Quantity & 0xFF);

            return request;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            var result = new Dictionary<Tuple<PointType, ushort>, ushort>();

            var p = (ModbusReadCommandParameters)CommandParameters;

            ushort startAddress = p.StartAddress;
            ushort quantity = p.Quantity;

            byte byteCount = response[8];

            int currentBit = 0;

            for (int i = 0; i < byteCount; i++)
            {
                for (int bit = 0; bit < 8; bit++)
                {
                    if (currentBit >= quantity)
                        break;

                    ushort value = (ushort)((response[9 + i] >> bit) & 0x01);

                    ushort address = (ushort)(startAddress + currentBit);

                    result[new Tuple<PointType, ushort>(PointType.DIGITAL_OUTPUT, address)] = value;

                    currentBit++;
                }
            }

            return result;
        }
    }
}