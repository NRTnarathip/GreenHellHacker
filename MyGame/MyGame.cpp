// MyGame.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
class MyGame {
public:
    MyGame* m_Instance;
    static void Start() {
        printf("On start game.");
    }
};
int main()
{
    std::cout << "Hello World!\n";
    auto myGame = new MyGame();
    while (true) {
        
    }
}

