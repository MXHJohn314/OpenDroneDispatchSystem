import os
from selenium import webdriver
from selenium.webdriver.common.by import By

def check_fleet_view(driver):
    page1 = driver.find_element(By.ID, "page1")
    page1.click()

    first_drone = driver.find_element(By.CLASS_NAME, "rz-button rz-button-md btn-primary rz-button-icon-only")
    first_drone.click()

    exit_button = driver.find_element(By.ID, "ADASDADASD")
    exit_button.click()

    data_connection = driver.find_element(By.CLASS_NAME, "connection-bar")
    data_connection.click()

def make_order(driver):
    page2 = driver.find_element(By.ID, "page2")

    page2.click()
    customer_input = driver.find_element(By.ID, "CUST_NAME")
    customer_input.send_keys("Customer Test1")

    address_input = driver.find_element(By.ID, "ADDRESS")
    address_input.send_keys("1640 W Colfax Ave, Denver, CO 80204")

    order_submit = driver.find_element(By.ID, "ORDER_CREATE_BTN")
    order_submit.click()

    order_id_input = driver.find_element(By.ID, "CANCEL_ORDER")
    order_id_input.send_keys("1")
    cancel_btn = driver.find_element(By.ID, "CANCEL_ORDER_BTN")
    cancel_btn.click()

    orders = driver.find_element(By.ID, "DRONE_FLEET_CARD")
    print(orders)

    drone_url = driver.find_element(By.ID, "DRONE_URL")
    drone_url.send_keys("test_drone2")
    add_drone = driver.find_element(By.ID, "ADD_DRONE_BTN")
    add_drone.click()
    drone_url2 = driver.find_element(By.ID, "DRONE_URL2")
    drone_url2.send_keys("1")
    remove_drone = driver.find_element(By.ID, "REMOVE_DRONE_URL")
    remove_drone.click()




def check_tracking(driver):
    page3 = driver.find_element(By.ID, "page3")
    page3.click()

    dropdown = driver.find_element(By.CLASS_NAME, "dropdown")
    dropdown.click()

    ready_btn = driver.find_element(By.XPATH, "//btn[text()='Ready']")
    ready_btn.click()
    dropdown.click()
    delivering_btn = driver.find_element(By.XPATH, "//btn[text()='Delivering']").click()
    dropdown.click()
    returning_btn = driver.find_element(By.XPATH, "//btn[text()='Returning']").click()
    dropdown.click()
    dead_btn = driver.find_element(By.XPATH, "//btn[text()='Dead']").click()
    dropdown.click()
    charging_btn = driver.find_element(By.XPATH, "//btn[text()='Charging']").click()
    dropdown.click()

    sat_view = driver.find_element(By.XPATH, "//button[@title='Show satellite imagery']").click()
    street_view = driver.find_element(By.XPATH, "//button[@title='Show street map']").click()





    zoom_in = driver.find_element(By.XPATH, "//button[@title='Zoom in']")
    zoom_in.click()

    zoom_out = driver.find_element(By.XPATH, "//button[@title='Zoom out']")
    zoom_out.click()

def main():
    os.environ['PATH'] += r"C:\Users\hdelg\OneDrive\Desktop\Capstone\proj_tests\chromedriver.exe"
    driver = webdriver.Chrome()
    driver.get('http://localhost:8081/')
    driver.implicitly_wait(30)

    # Checks the order put then the tracking page.
    make_order(driver)
    check_tracking(driver)
    check_fleet_view(driver)


if __name__ == "__main__":
    main()