#include "screen.h"


void Screen::Init(Orientation_t _orientation)
{
    LCD_CS_GPIO_Port->BSRR = (uint32_t) LCD_CS_Pin << 16U;
    ChipSelect(true);

    HAL_Delay(5);
    Reset(true);
    HAL_Delay(10);
    Reset(false);
    HAL_Delay(120);

    /* Initial Sequence */
    WriteCommand(0xEF);

    WriteCommand(0xEB);
    Write1Byte(0x14);

    WriteCommand(0xFE);
    WriteCommand(0xEF);

    WriteCommand(0xEB);
    Write1Byte(0x14);

    WriteCommand(0x84);
    Write1Byte(0x40);

    WriteCommand(0x85);
    Write1Byte(0xFF);

    WriteCommand(0x86);
    Write1Byte(0xFF);

    WriteCommand(0x87);
    Write1Byte(0xFF);

    WriteCommand(0x88);
    Write1Byte(0x0A);

    WriteCommand(0x89);
    Write1Byte(0x21);

    WriteCommand(0x8A);
    Write1Byte(0x00);

    WriteCommand(0x8B);
    Write1Byte(0x80);

    WriteCommand(0x8C);
    Write1Byte(0x01);

    WriteCommand(0x8D);
    Write1Byte(0x01);

    WriteCommand(0x8E);
    Write1Byte(0xFF);

    WriteCommand(0x8F);
    Write1Byte(0xFF);


    WriteCommand(0xB6);
    Write1Byte(0x00);
    Write1Byte(0x00);

    WriteCommand(0x36);
    switch (_orientation)
    {
        case DEGREE_0:
            Write1Byte(0x18);
            break;
        case DEGREE_90:
            Write1Byte(0x28);
            break;
        case DEGREE_180:
            Write1Byte(0x48);
            break;
        case DEGREE_270:
            Write1Byte(0x88);
            break;
    }

    WriteCommand(0x3A); // COLOR_MODE
    switch (colorMode)
    {
        case BIT_12:
            Write1Byte(0x03);
            break;
        case BIT_16:
            Write1Byte(0x05);
            break;
        case BIT_18:
            Write1Byte(0x06);
            break;
    }

    WriteCommand(0x90);
    Write1Byte(0x08);
    Write1Byte(0x08);
    Write1Byte(0x08);
    Write1Byte(0x08);

    WriteCommand(0xBD);
    Write1Byte(0x06);

    WriteCommand(0xBC);
    Write1Byte(0x00);

    WriteCommand(0xFF);
    Write1Byte(0x60);
    Write1Byte(0x01);
    Write1Byte(0x04);

    WriteCommand(0xC3);
    Write1Byte(0x13);
    WriteCommand(0xC4);
    Write1Byte(0x13);

    WriteCommand(0xC9);
    Write1Byte(0x22);

    WriteCommand(0xBE);
    Write1Byte(0x11);

    WriteCommand(0xE1);
    Write1Byte(0x10);
    Write1Byte(0x0E);

    WriteCommand(0xDF);
    Write1Byte(0x21);
    Write1Byte(0x0c);
    Write1Byte(0x02);

    WriteCommand(0xF0);
    Write1Byte(0x45);
    Write1Byte(0x09);
    Write1Byte(0x08);
    Write1Byte(0x08);
    Write1Byte(0x26);
    Write1Byte(0x2A);

    WriteCommand(0xF1);
    Write1Byte(0x43);
    Write1Byte(0x70);
    Write1Byte(0x72);
    Write1Byte(0x36);
    Write1Byte(0x37);
    Write1Byte(0x6F);

    WriteCommand(0xF2);
    Write1Byte(0x45);
    Write1Byte(0x09);
    Write1Byte(0x08);
    Write1Byte(0x08);
    Write1Byte(0x26);
    Write1Byte(0x2A);

    WriteCommand(0xF3);
    Write1Byte(0x43);
    Write1Byte(0x70);
    Write1Byte(0x72);
    Write1Byte(0x36);
    Write1Byte(0x37);
    Write1Byte(0x6F);

    WriteCommand(0xED);
    Write1Byte(0x1B);
    Write1Byte(0x0B);

    WriteCommand(0xAE);
    Write1Byte(0x77);

    WriteCommand(0xCD);
    Write1Byte(0x63);

    WriteCommand(0x70);
    Write1Byte(0x07);
    Write1Byte(0x07);
    Write1Byte(0x04);
    Write1Byte(0x0E);
    Write1Byte(0x0F);
    Write1Byte(0x09);
    Write1Byte(0x07);
    Write1Byte(0x08);
    Write1Byte(0x03);

    WriteCommand(0xE8);
    Write1Byte(0x34);

    WriteCommand(0x62);
    Write1Byte(0x18);
    Write1Byte(0x0D);
    Write1Byte(0x71);
    Write1Byte(0xED);
    Write1Byte(0x70);
    Write1Byte(0x70);
    Write1Byte(0x18);
    Write1Byte(0x0F);
    Write1Byte(0x71);
    Write1Byte(0xEF);
    Write1Byte(0x70);
    Write1Byte(0x70);

    WriteCommand(0x63);
    Write1Byte(0x18);
    Write1Byte(0x11);
    Write1Byte(0x71);
    Write1Byte(0xF1);
    Write1Byte(0x70);
    Write1Byte(0x70);
    Write1Byte(0x18);
    Write1Byte(0x13);
    Write1Byte(0x71);
    Write1Byte(0xF3);
    Write1Byte(0x70);
    Write1Byte(0x70);

    WriteCommand(0x64);
    Write1Byte(0x28);
    Write1Byte(0x29);
    Write1Byte(0xF1);
    Write1Byte(0x01);
    Write1Byte(0xF1);
    Write1Byte(0x00);
    Write1Byte(0x07);

    WriteCommand(0x66);
    Write1Byte(0x3C);
    Write1Byte(0x00);
    Write1Byte(0xCD);
    Write1Byte(0x67);
    Write1Byte(0x45);
    Write1Byte(0x45);
    Write1Byte(0x10);
    Write1Byte(0x00);
    Write1Byte(0x00);
    Write1Byte(0x00);

    WriteCommand(0x67);
    Write1Byte(0x00);
    Write1Byte(0x3C);
    Write1Byte(0x00);
    Write1Byte(0x00);
    Write1Byte(0x00);
    Write1Byte(0x01);
    Write1Byte(0x54);
    Write1Byte(0x10);
    Write1Byte(0x32);
    Write1Byte(0x98);

    WriteCommand(0x74);
    Write1Byte(0x10);
    Write1Byte(0x85);
    Write1Byte(0x80);
    Write1Byte(0x00);
    Write1Byte(0x00);
    Write1Byte(0x4E);
    Write1Byte(0x00);

    WriteCommand(0x98);
    Write1Byte(0x3e);
    Write1Byte(0x07);

    WriteCommand(0x35);
    WriteCommand(0x21);

    WriteCommand(0x11);
    HAL_Delay(120);
    WriteCommand(0x29);
    HAL_Delay(20);

    ChipSelect(false);

    SetBackLight(1);
}


void Screen::SetWindow(uint16_t _startX, uint16_t _endX, uint16_t _startY, uint16_t _endY)
{
    ChipSelect(true);

    uint8_t data[4];

    WriteCommand(0x2A); // COL_ADDR_SET
    data[0] = (_startX >> 8) & 0xFF;
    data[1] = _startX & 0xFF;
    data[2] = (_endX >> 8) & 0xFF;
    data[3] = _endX & 0xFF;
    WriteData(data, sizeof(data));

    WriteCommand(0x2B); // ROW_ADDR_SET
    data[0] = (_startY >> 8) & 0xFF;
    data[1] = _startY & 0xFF;
    data[2] = (_endY >> 8) & 0xFF;
    data[3] = _endY & 0xFF;
    WriteData(data, sizeof(data));

    ChipSelect(false);
}


void Screen::WriteFrameBuffer(uint8_t* _buffer, uint32_t _len, bool _isAppend)
{
    isBusy = true;

    ChipSelect(true);
    _isAppend ?
    WriteCommand(0x3C) : // MEM_WR_CONT
    WriteCommand(0x2C);  // MEM_WR
    WriteData(_buffer, _len, true);

    // need to wait DMA transmit finish if used DMA
    ChipSelect(false);
}


void Screen::ChipSelect(bool _enable)
{
//    _enable ? LCD_CS_GPIO_Port->BSRR = (uint32_t) LCD_CS_Pin << 16U :
//            LCD_CS_GPIO_Port->BSRR = LCD_CS_Pin;
}


void Screen::Reset(bool _enable)
{
    _enable ? LCD_RES_GPIO_Port->BSRR = (uint32_t) LCD_RES_Pin << 16U :
            LCD_RES_GPIO_Port->BSRR = LCD_RES_Pin;
}


void Screen::SetDataOrCommand(bool _isData)
{
    _isData ? LCD_DC_GPIO_Port->BSRR = LCD_DC_Pin :
            LCD_DC_GPIO_Port->BSRR = (uint32_t) LCD_DC_Pin << 16U;
}


void Screen::WriteCommand(uint8_t _cmd)
{
    SetDataOrCommand(false);
    HAL_SPI_Transmit(spi, &_cmd, 1, 100);
}


void Screen::Write1Byte(uint8_t _data)
{
    SetDataOrCommand(true);
    HAL_SPI_Transmit(spi, &_data, 1, 100);
}


void Screen::WriteData(uint8_t* _data, uint32_t _len, bool _useDma)
{
    SetDataOrCommand(true);
    _useDma ? HAL_SPI_Transmit_DMA(spi, _data, _len) :
    HAL_SPI_Transmit(spi, _data, _len, 100);
}


void Screen::SetBackLight(float _val)
{
    if (_val < 0) _val = 0;
    else if (_val > 1.0f) _val = 1.0f;

    HAL_GPIO_WritePin(GPIOA, GPIO_PIN_11, GPIO_PIN_SET);
}


