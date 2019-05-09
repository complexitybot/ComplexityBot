import React from "react";

export class CreateLevelForm extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            topic: 'Сложность алгоритмов',
            level: ''
        };

        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    handleChange(event) {
        const target = event.target;
        const name = target.name;

        this.setState({
            [name]: value
        });
    }

    handleSubmit(event) {
        alert('Был создан пустой Level: ' + this.state.kevek);
        event.preventDefault();
    }

    render() {
        return (
            <form onSubmit={this.handleSubmit}>
                <h3>Добавление Level</h3>
                <label>
                    Выберите Topic, в который хотите добавить Level:
                    <select name="topic" value={this.state.topic} onChange={this.handleChange}>
                        <option value="Сложность алгоритмов">Сложность алгоритмов</option>
                    </select>
                </label>
                <br/>
                <label>
                    Имя Level, который хотите добавить:
                    <textarea name="level" value={this.state.level} onChange={this.handleChange} />
                </label>
                <input type="submit" value="Create Level" />
            </form>
        );
    }
}