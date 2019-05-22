import React from "react";
import '../../styles/EditorForm.css'

export class DeleteGeneratorForm extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            topic: 'Cложность алгоритмов',
            level: 'Циклы',
            generator: 'Generator 1',
        };

        this.handleInputChange = this.handleInputChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    handleInputChange(event) {
        const target = event.target;
        const name = target.name;
        const value = event.target.value;
        this.setState({
            [name]: value
        });
    }

    handleSubmit(event) {
        alert('Вы удалили Generator: ' + this.state.generator);
        event.preventDefault();
    }

    render() {
        return (
            <form onSubmit={this.handleSubmit}>
                <h3>Удаление Generator</h3>
                <label>
                    <p>Выберите Topic, из которого хотите удалить Generator:</p>
                    <select name="topic" value={this.state.topic} onChange={this.handleInputChange}>
                        <option value="Сложность алгоритмов">Сложность алгоритмов</option>
                    </select>
                </label>
                <br/>
                <label>
                    <p>Выберите Level, из которого хотите удалить Generator:</p>
                    <select name="level" value={this.state.level} onChange={this.handleInputChange}>
                        <option value="Циклы">Циклы</option>
                        <option value="Двойные циклы">Двойные циклы</option>
                    </select>
                </label>
                <br/>
                <label>
                    <p>Выберите Generator, который хотите удалить:</p>
                    <select name="generator" value={this.state.generator} onChange={this.handleInputChange}>
                        <option value="generator_1">Generator 1</option>
                    </select>
                </label>
                <br/><br/>
                <input className="button2" type="submit" value="Submit" />
            </form>
        );
    }
}