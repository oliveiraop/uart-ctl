#include "uartController.h"

#include <iostream>
#include <fcntl.h>
#include <termios.h>
#include <unistd.h>
#include <cstring>

UARTController::UARTController(const char* device) : uart(-1), device(device) {}

bool UARTController::openUART() {
    uart = open(device, O_RDWR | O_NOCTTY | O_NDELAY);
    if (uart == -1) {
        std::cerr << "Erro ao abrir a porta UART." << std::endl;
        return false;
    }

    struct termios options;
    tcgetattr(uart, &options);
    options.c_cflag = B115200 | CS8 | CLOCAL | CREAD;
    options.c_iflag = 0;
    options.c_oflag = 0;
    options.c_lflag = 0;
    tcflush(uart, TCIFLUSH);
    tcsetattr(uart, TCSANOW, &options);

    return true;
}

void UARTController::closeUART() {
    if (uart != -1) {
        close(uart);
        uart = -1;
    }
}

bool UARTController::send(const char* data) {
    if (uart == -1) {
        std::cerr << "A porta UART não está aberta." << std::endl;
        return false;
    }

    ssize_t bytesWritten = write(uart, data, strlen(data));
    if (bytesWritten == -1) {
        std::cerr << "Erro ao enviar dados pela UART." << std::endl;
        return false;
    }

    return true;
}

bool UARTController::receive() {
    if (uart == -1) {
        std::cerr << "A porta UART não está aberta." << std::endl;
        return false;
    }

    char buffer[256];
    ssize_t bytesRead = read(uart, buffer, sizeof(buffer) - 1);
    if (bytesRead == -1) {
        std::cerr << "Erro ao ler dados da UART." << std::endl;
        return false;
    }

    buffer[bytesRead] = '\0';
    std::cout << "Dados recebidos: " << buffer << std::endl;

    return true;
}
