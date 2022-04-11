//https://github.com/GreatWall51/stm32f0_flash

#include "stm32f0xx_hal.h"
#include "flash.h"

#define BUILD_UINT16(loByte, hiByte) \
          ((uint16_t)(((loByte) & 0x00FF) + (((hiByte) & 0x00FF) << 8)))

#define BUILD_UINT32(Byte0, Byte1, Byte2, Byte3) \
          ((uint32_t)((uint32_t)((Byte0) & 0x00FF) \
          + ((uint32_t)((Byte1) & 0x00FF) << 8) \
          + ((uint32_t)((Byte2) & 0x00FF) << 16) \
          + ((uint32_t)((Byte3) & 0x00FF) << 24)))


uint32_t flash_read(uint32_t address, uint8_t* pdata, uint32_t size)
{
    uint32_t read_index = 0;
    uint8_t value;
    uint32_t start_addr;
    uint32_t end_addr;

    if (!pdata || size < 1)
    {
        return 0;
    }
    start_addr = address;
    end_addr = start_addr + size;
    if (start_addr < FLASH_USER_START_ADDR || end_addr > FLASH_USER_END_ADDR)
    {
        return 0;
    }

    read_index = 0;
    while (read_index < size)
    {
        value = *(__IO uint8_t*) start_addr;
        start_addr = start_addr + 1;
        *(pdata + read_index) = value;
        read_index++;
    }
    return read_index;
}


FLASH_ERROR_CODE_E flash_write(uint32_t address, const uint8_t* pdata, uint32_t size)
{
    HAL_StatusTypeDef result = HAL_ERROR;

    uint32_t end_addr = 0;
    uint32_t start_addr;
    uint32_t word_num;
    uint8_t half_word_num;
    uint8_t byte_num;
    uint32_t write_index = 0;

    if ((!pdata) || (size < 1))
    {
        return FLASH_PARAM_ERROR;
    }
    word_num = (size >> 2);
    half_word_num = (size % 4) >> 1;
    byte_num = (size % 2);
    start_addr = address;
    end_addr = (start_addr + size);
    if (start_addr < FLASH_USER_START_ADDR || end_addr > FLASH_USER_END_ADDR)
    {
        return FLASH_ADDR_ERROR;
    }
    HAL_FLASH_Unlock();
    write_index = 0;
    while (write_index < word_num)
    {
        result = HAL_FLASH_Program(FLASH_TYPEPROGRAM_WORD, start_addr,
                                   BUILD_UINT32 (*(pdata), *(pdata + 1), *(pdata + 2), *(pdata + 3)));
        if (HAL_OK == result)
        {
            start_addr = start_addr + 4;
            pdata = pdata + 4;
            write_index++;
        } else
        {
            return FLASH_WRITE_WORD_ERROR;
        }
    }
    write_index = 0;
    while (write_index < half_word_num)
    {
        result = HAL_FLASH_Program(FLASH_TYPEPROGRAM_HALFWORD, start_addr,
                                   BUILD_UINT16 (*(pdata), *(pdata + 1)));
        if (HAL_OK == result)
        {
            start_addr = start_addr + 2;
            pdata = pdata + 2;
            write_index++;
        } else
        {
            return FLASH_WRITE_HALF_WORD_ERROR;
        }
    }
    write_index = 0;
    while (write_index < byte_num)
    {
        result = HAL_FLASH_Program(FLASH_TYPEPROGRAM_HALFWORD, start_addr, BUILD_UINT16 (*(pdata), 0xFFFF));
        if (HAL_OK == result)
        {
            start_addr = start_addr + 2;
            pdata = pdata + 2;
            write_index++;
        } else
        {
            return FLASH_WRITE_BYTE_ERROR;
        }
    }
    HAL_FLASH_Lock();
    return FLASH_SUCCESS;
}


FLASH_ERROR_CODE_E flash_erase(uint32_t start_addr, uint32_t end_addr)
{
    static FLASH_EraseInitTypeDef EraseInitStruct;
    uint32_t PageError = 0;

    if ((start_addr > end_addr) || (start_addr < FLASH_USER_START_ADDR) || (end_addr > FLASH_USER_END_ADDR))
    {
        return FLASH_ADDR_ERROR;
    }
    HAL_FLASH_Unlock();

    EraseInitStruct.TypeErase = FLASH_TYPEERASE_PAGES;
    EraseInitStruct.PageAddress = start_addr;
    EraseInitStruct.NbPages = (end_addr - start_addr + (FLASH_PAGE_SIZE - 1)) / FLASH_PAGE_SIZE;
    if (HAL_FLASHEx_Erase(&EraseInitStruct, &PageError) != HAL_OK)
    {
        HAL_FLASH_Lock();
        return FLASH_ERASE_ERROR;
    }
    HAL_FLASH_Lock();
    return FLASH_SUCCESS;
}


uint32_t flash_read_page(uint8_t page_no, uint32_t offect, uint8_t* pdata, uint32_t size)
{
    uint32_t result = 0;
    uint32_t addr;

    addr = PAGE_TO_ADDR(page_no);
    addr += offect;
    result = flash_read(addr, pdata, size);
    return result;
}


uint32_t flash_write_page(uint8_t page_no, uint32_t offect, const uint8_t* pdata, uint32_t size)
{
    uint32_t result = 0;
    uint32_t addr;

    addr = PAGE_TO_ADDR(page_no);
    addr += offect;
    result = flash_write(addr, pdata, size);
    return result;
}


FLASH_ERROR_CODE_E flash_erase_page(uint32_t start_page, uint16_t page_cnt)
{
    static FLASH_EraseInitTypeDef EraseInitStruct;
    uint32_t PageError = 0;

    HAL_FLASH_Unlock();

    EraseInitStruct.TypeErase = FLASH_TYPEERASE_PAGES;
    EraseInitStruct.PageAddress = PAGE_TO_ADDR(start_page);
    EraseInitStruct.NbPages = page_cnt;

    if (HAL_FLASHEx_Erase(&EraseInitStruct, &PageError) != HAL_OK)
    {
        HAL_FLASH_Lock();
        return FLASH_ERASE_ERROR;
    }

    HAL_FLASH_Lock();

    return FLASH_SUCCESS;
}
