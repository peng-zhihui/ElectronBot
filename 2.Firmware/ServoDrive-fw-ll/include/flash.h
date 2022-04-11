//https://github.com/GreatWall51/stm32f0_flash

#ifndef  __FLASH__
#define  __FLASH__

#include <stdint.h>

#define FLASH_PAGE_TO_ADDR(page)          ((uint32_t)(FLASH_BASE+(FLASH_PAGE_SIZE)*(page)))
#define ADDR_TO_FLASH_PAGE(addr)          (((addr)-FLASH_BASE)/(FLASH_PAGE_SIZE))

#define FLASH_USER_START_ADDR       0x08008000UL-0x100U // 256B
#define FLASH_USER_END_ADDR         0x08007FFFUL


typedef enum
{
    FLASH_SUCCESS = 0,
    FLASH_PARAM_ERROR,
    FLASH_ADDR_ERROR,
    FLASH_WRITE_WORD_ERROR,
    FLASH_WRITE_HALF_WORD_ERROR,
    FLASH_WRITE_BYTE_ERROR,
    FLASH_READ_ERROR,
    FLASH_ERASE_ERROR,
} FLASH_ERROR_CODE_E;

#define ADDR_TO_PAGE(addr)    (((addr)-FLASH_USER_START_ADDR)/FLASH_PAGE_SIZE)
#define PAGE_TO_ADDR(pag_no)  ((pag_no)*FLASH_PAGE_SIZE+FLASH_USER_START_ADDR)

FLASH_ERROR_CODE_E flash_write(uint32_t address, const uint8_t* pdata, uint32_t size);
uint32_t flash_read(uint32_t address, uint8_t* pdata, uint32_t size);
FLASH_ERROR_CODE_E flash_erase(uint32_t start_addr, uint32_t end_addr);

uint32_t flash_read_page(uint8_t sec_no, uint32_t offect, uint8_t* pdata, uint32_t size);
uint32_t flash_write_page(uint8_t sec_no, uint32_t offect, const uint8_t* pdata, uint32_t size);
FLASH_ERROR_CODE_E flash_erase_page(uint32_t start_page, uint16_t page_cnt);

#endif
