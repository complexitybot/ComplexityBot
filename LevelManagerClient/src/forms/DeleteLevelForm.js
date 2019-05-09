import React from "react";

export class DeleteLevelForm extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            topic: 'Cложность алгоритмов',
            level: 'Циклы'};

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
        alert('Вы удалили Level: ' + this.state.level);
        event.preventDefault();
    }

    render() {
        return (
            <form onSubmit={this.handleSubmit}>
                <h3>Удаление Level</h3>
                <label>
                    Выберите Topic, из которого хотите удалить Level:
                    <select name="topic" value={this.state.topic} onChange={this.handleChange}>
                        <option value="Сложность алгоритмов">Сложность алгоритмов</option>
                    </select>
                </label>
                <br/>
                <label>
                    Выберите Level, который хотите удалить:
                    <select name="level" value={this.state.level} onChange={this.handleChange}>
                        <option value="Циклы">Циклы</option>
                        <option value="Двойные циклы">Двойные циклы</option>
                    </select>
                </label>
                <input type="submit" value="Submit" />
            </form>
        );
    }
}