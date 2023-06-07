import React, { useContext, useState } from "react";
import { CartContext } from "../../store/cartContext";
import classes from "./Cart.module.css";
import { Link, useNavigate } from "react-router-dom";
import { useEffect } from "react";

const Cart = () => {
  const navigator = useNavigate();
  const cartContext = useContext(CartContext);
  const [address, setAddress] = useState("");
  const [comment, setComment] = useState("");

  useEffect(() => {
    setAddress(cartContext.address);
    setComment(cartContext.comment);
  }, [cartContext.address, cartContext.comment]);

  const handleRemoveItem = (itemId) => {
    cartContext.removeFromCart(itemId);
  };

  const handleIncrementChange = (itemId) => {
    cartContext.increaseItemQuantity(itemId);
  };

  const handleDecrementChange = (itemId) => {
    cartContext.decreaseItemQuantity(itemId);
  };

  const calculateTotalPrice = () => {
    let totalPrice = 0;
    for (const item of cartContext.cartItems) {
      totalPrice += item.price * item.quantity;
    }
    return totalPrice;
  };

  const handlePurchase = async () => {
    if (!address || address.trim() === "") {
      alert("Address is reqired");
      return;
    }

    cartContext.setCartAddress(address);
    cartContext.setCartComment(comment);
    navigator('/confirm_purchase');
  };

  return (
    <div className={classes.shoppingCart}>
      <h2>Shopping Cart</h2>
      <ul className={classes.itemList}>
        {cartContext.cartItems.map((item) => (
          <li key={item.id} className={classes.item}>
            <div className={classes.itemNamePrice}>
              <span className={classes.itemName}>{item.name}</span>
              <div className={classes.itemPrice}>
                Price: ${item.price * item.quantity}
              </div>
            </div>
            <div className={classes.itemActions}>
              <button
                className={classes.decrementButton}
                onClick={() => handleDecrementChange(item.id)}
                disabled={item.quantity === 1}
              >
                -
              </button>
              <input
                type="text"
                className={classes.itemQuantity}
                value={item.quantity}
                readOnly
              />
              <button
                className={classes.incrementButton}
                onClick={() => handleIncrementChange(item.id)}
                disabled={item.quantity === item.amount}
              >
                +
              </button>
            </div>
            <button
              className={classes.removeButton}
              onClick={() => handleRemoveItem(item.id)}
            >
              Remove
            </button>
          </li>
        ))}
      </ul>
      <div className={classes.inputFields}>
        <label htmlFor="address">Address:</label>
        <input
          type="text"
          id="address"
          value={address}
          onChange={(e) => setAddress(e.target.value)}
          required
        />
        <label htmlFor="comment">Comment:</label>
        <input
          type="text"
          id="comment"
          value={comment}
          onChange={(e) => setComment(e.target.value)}
        />
      </div>
      <div className={classes.totalPrice}>
        Total Price: ${calculateTotalPrice()}
      </div>
      <div className={classes.note}>
        <p> This price does not include delivery fee.</p>
      </div>
      <div className={classes.linkButton}>
        <Link className={classes.link} to="/create_new_order" />
        <button className={classes.confirmPurchase} onClick={handlePurchase}>
          Confirm Purchase
        </button>
      </div>
    </div>
  );
};

export default Cart;
