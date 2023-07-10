#include "uartController.h"

#include <iostream>

int main(int argc, char* argv[]) {
    if (argc < 2) {
        std::cerr << "Informe o dispositivo UART." << std::endl;
        std::cerr << "Exemplo: ./uart-ctl /dev/ttymxc0" << std::endl;
        return 1;
    }

    const char* device = argv[1];

    UARTController uartController(device);

    if (!uartController.openUART()) {
        return 1;
    }

    const char* data = "Hello, UART!";
    if (!uartController.send(data)) {
        uartController.closeUART();
        return 1;
    }

    if (!uartController.receive()) {
        uartController.closeUART();
        return 1;
    }

    uartController.closeUART();

    return 0;
}
