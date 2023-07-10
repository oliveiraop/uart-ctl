#ifndef UART_CONTROLLER_H
#define UART_CONTROLLER_H

class UARTController {
public:
    UARTController(const char* device);

    bool openUART();
    void closeUART();
    bool send(const char* data);
    bool receive();

private:
    int uart;
    const char* device;
};

#endif
